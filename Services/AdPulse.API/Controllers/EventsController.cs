using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services.Elasticsearch;
using System.Security.Claims;

namespace AdPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly AdPulseDbContext _context;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        AdPulseDbContext context,
        IElasticsearchService elasticsearchService,
        ILogger<EventsController> logger)
    {
        _context = context;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    private Guid? TryGetTenantId()
    {
        var claim = User.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(claim) && Guid.TryParse(claim, out var tenantId))
        {
            return tenantId;
        }

        if (Request.Headers.TryGetValue("X-Tenant-ID", out var headerVal) && Guid.TryParse(headerVal, out var headerTenantId))
        {
            return headerTenantId;
        }

        return null;
    }

    /// <summary>
    /// Ingest a single ad event (impression, click, conversion, video_view)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IngestEvent([FromBody] IngestEventRequest request)
    {
        var tenantId = TryGetTenantId();
        
        // If not passed in headers, look up campaign to get tenant ID
        if (!tenantId.HasValue)
        {
            var campaign = await _context.Campaigns.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == request.CampaignId);
            if (campaign == null)
            {
                return BadRequest(new { message = "Campaign ID not found" });
            }
            tenantId = campaign.TenantId;
        }

        var adEvent = new AdEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId.Value,
            CampaignId = request.CampaignId,
            AdGroupId = request.AdGroupId,
            CreativeId = request.CreativeId,
            EventType = request.EventType,
            EventTime = request.EventTime ?? DateTime.UtcNow,
            UserId = request.UserId,
            SessionId = request.SessionId,
            IpAddress = request.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = request.UserAgent ?? Request.Headers.UserAgent.ToString(),
            DeviceType = request.DeviceType ?? "Desktop",
            Country = request.Country ?? "United States",
            City = request.City ?? "New York",
            Referrer = request.Referrer,
            ConversionValue = request.ConversionValue,
            CustomData = request.CustomData
        };

        _context.AdEvents.Add(adEvent);

        // Update campaign spent amount or stats if conversion/click
        if (request.EventType == AdEventType.Click)
        {
            var campaign = await _context.Campaigns.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == request.CampaignId);
            if (campaign != null)
            {
                campaign.SpentAmount += 0.50m; // Approximate CPC
            }
        }

        await _context.SaveChangesAsync();

        // Index in Elasticsearch
        await _elasticsearchService.IndexEventAsync(adEvent);

        _logger.LogInformation("AdEvent ingested: {EventId}, Type: {Type}, Campaign: {CampaignId}",
            adEvent.Id, adEvent.EventType, adEvent.CampaignId);

        var dto = new EventDto(
            adEvent.Id,
            adEvent.TenantId,
            adEvent.CampaignId,
            adEvent.AdGroupId,
            adEvent.CreativeId,
            adEvent.EventType,
            adEvent.EventTime,
            adEvent.UserId,
            adEvent.DeviceType,
            adEvent.Country,
            adEvent.City,
            adEvent.ConversionValue
        );

        return CreatedAtAction(nameof(IngestEvent), new { id = adEvent.Id }, dto);
    }

    /// <summary>
    /// Bulk ingest ad events from event processing queues
    /// </summary>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(BatchIngestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IngestBatch([FromBody] BatchIngestRequest request)
    {
        if (request.Events == null || request.Events.Count == 0)
        {
            return BadRequest(new { message = "Events list cannot be empty" });
        }

        int successful = 0;
        int failed = 0;
        var errors = new List<string>();
        var eventsToAdd = new List<AdEvent>();

        var defaultTenantId = TryGetTenantId();

        // Cache campaigns to avoid round trips
        var campaignIds = request.Events.Select(e => e.CampaignId).Distinct().ToList();
        var campaigns = await _context.Campaigns.IgnoreQueryFilters()
            .Where(c => campaignIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c);

        foreach (var ev in request.Events)
        {
            try
            {
                Guid tenantId;
                if (campaigns.TryGetValue(ev.CampaignId, out var camp))
                {
                    tenantId = camp.TenantId;
                    if (ev.EventType == AdEventType.Click)
                    {
                        camp.SpentAmount += 0.50m;
                    }
                }
                else if (defaultTenantId.HasValue)
                {
                    tenantId = defaultTenantId.Value;
                }
                else
                {
                    failed++;
                    errors.Add($"Campaign {ev.CampaignId} not found");
                    continue;
                }

                var adEvent = new AdEvent
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CampaignId = ev.CampaignId,
                    AdGroupId = ev.AdGroupId,
                    CreativeId = ev.CreativeId,
                    EventType = ev.EventType,
                    EventTime = ev.EventTime ?? DateTime.UtcNow,
                    UserId = ev.UserId,
                    SessionId = ev.SessionId,
                    IpAddress = ev.IpAddress,
                    UserAgent = ev.UserAgent,
                    DeviceType = ev.DeviceType ?? "Desktop",
                    Country = ev.Country ?? "United States",
                    City = ev.City,
                    Referrer = ev.Referrer,
                    ConversionValue = ev.ConversionValue,
                    CustomData = ev.CustomData
                };

                eventsToAdd.Add(adEvent);
                successful++;
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add(ex.Message);
            }
        }

        if (eventsToAdd.Count > 0)
        {
            _context.AdEvents.AddRange(eventsToAdd);
            await _context.SaveChangesAsync();

            // Index in Elasticsearch in background
            _ = Task.Run(async () =>
            {
                foreach (var ev in eventsToAdd)
                {
                    await _elasticsearchService.IndexEventAsync(ev);
                }
            });
        }

        _logger.LogInformation("Batch events ingested: {Count} successful, {Failed} failed", successful, failed);

        return Ok(new BatchIngestResponse(successful, failed, errors));
    }

    /// <summary>
    /// Get recent events for tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentEvents(
        [FromQuery] Guid? campaignId,
        [FromQuery] int limit = 50)
    {
        var tenantId = TryGetTenantId();
        if (!tenantId.HasValue)
        {
            return Unauthorized(new { message = "Authentication or Tenant ID required" });
        }

        var query = _context.AdEvents.Where(e => e.TenantId == tenantId.Value);

        if (campaignId.HasValue)
        {
            query = query.Where(e => e.CampaignId == campaignId.Value);
        }

        var events = await query
            .OrderByDescending(e => e.EventTime)
            .Take(Math.Min(limit, 200))
            .Select(e => new EventDto(
                e.Id,
                e.TenantId,
                e.CampaignId,
                e.AdGroupId,
                e.CreativeId,
                e.EventType,
                e.EventTime,
                e.UserId,
                e.DeviceType,
                e.Country,
                e.City,
                e.ConversionValue
            ))
            .ToListAsync();

        return Ok(events);
    }
}
