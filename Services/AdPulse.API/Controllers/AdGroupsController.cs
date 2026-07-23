using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using System.Security.Claims;

namespace AdPulse.API.Controllers;

[ApiController]
[Authorize]
public class AdGroupsController : ControllerBase
{
    private readonly AdPulseDbContext _context;
    private readonly ILogger<AdGroupsController> _logger;

    public AdGroupsController(AdPulseDbContext context, ILogger<AdGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private Guid GetTenantId()
    {
        return Guid.Parse(User.FindFirst("tenant_id")?.Value 
            ?? throw new UnauthorizedAccessException("Tenant ID not found"));
    }

    /// <summary>
    /// Get all ad groups for a campaign
    /// </summary>
    [HttpGet("api/campaigns/{campaignId}/adgroups")]
    [ProducesResponseType(typeof(List<AdGroupListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdGroups(Guid campaignId)
    {
        var tenantId = GetTenantId();
        var adGroups = await _context.AdGroups
            .Where(ag => ag.CampaignId == campaignId && ag.TenantId == tenantId)
            .OrderByDescending(ag => ag.CreatedAt)
            .Select(ag => new AdGroupListDto(
                ag.Id,
                ag.Name,
                ag.Status,
                ag.BiddingStrategy,
                ag.BidAmount,
                ag.CreatedAt
            ))
            .ToListAsync();

        return Ok(adGroups);
    }

    /// <summary>
    /// Get specific ad group with its creatives
    /// </summary>
    [HttpGet("api/adgroups/{id}")]
    [ProducesResponseType(typeof(AdGroupDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdGroup(Guid id)
    {
        var tenantId = GetTenantId();
        var adGroup = await _context.AdGroups
            .Include(ag => ag.Creatives)
            .FirstOrDefaultAsync(ag => ag.Id == id && ag.TenantId == tenantId);

        if (adGroup == null)
        {
            return NotFound(new { message = "Ad group not found" });
        }

        var detail = new AdGroupDetailDto(
            adGroup.Id,
            adGroup.Name,
            adGroup.Status,
            adGroup.BiddingStrategy,
            adGroup.BidAmount,
            adGroup.TargetingRules,
            adGroup.CreatedAt,
            adGroup.UpdatedAt,
            adGroup.Creatives.Select(c => new CreativeListDto(
                c.Id,
                c.Name,
                c.Type,
                c.Status,
                c.Headline,
                c.ImageUrl,
                c.CreatedAt
            )).ToList()
        );

        return Ok(detail);
    }

    /// <summary>
    /// Create a new ad group within a campaign
    /// </summary>
    [HttpPost("api/campaigns/{campaignId}/adgroups")]
    [ProducesResponseType(typeof(AdGroupDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAdGroup(Guid campaignId, [FromBody] CreateAdGroupRequest request)
    {
        var tenantId = GetTenantId();

        // Verify campaign exists and belongs to tenant
        var campaignExists = await _context.Campaigns.AnyAsync(c => c.Id == campaignId && c.TenantId == tenantId);
        if (!campaignExists)
        {
            return NotFound(new { message = "Campaign not found" });
        }

        var adGroup = new AdGroup
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CampaignId = campaignId,
            Name = request.Name,
            Status = AdGroupStatus.Active,
            BiddingStrategy = request.BiddingStrategy,
            BidAmount = request.BidAmount,
            TargetingRules = request.TargetingRules,
            CreatedAt = DateTime.UtcNow
        };

        _context.AdGroups.Add(adGroup);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Ad group created: {AdGroupId} for campaign {CampaignId}", adGroup.Id, campaignId);

        var detail = new AdGroupDetailDto(
            adGroup.Id,
            adGroup.Name,
            adGroup.Status,
            adGroup.BiddingStrategy,
            adGroup.BidAmount,
            adGroup.TargetingRules,
            adGroup.CreatedAt,
            adGroup.UpdatedAt,
            new List<CreativeListDto>()
        );

        return CreatedAtAction(nameof(GetAdGroup), new { id = adGroup.Id }, detail);
    }

    /// <summary>
    /// Update an existing ad group
    /// </summary>
    [HttpPut("api/adgroups/{id}")]
    [ProducesResponseType(typeof(AdGroupDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAdGroup(Guid id, [FromBody] UpdateAdGroupRequest request)
    {
        var tenantId = GetTenantId();
        var adGroup = await _context.AdGroups
            .Include(ag => ag.Creatives)
            .FirstOrDefaultAsync(ag => ag.Id == id && ag.TenantId == tenantId);

        if (adGroup == null)
        {
            return NotFound(new { message = "Ad group not found" });
        }

        if (request.Name != null) adGroup.Name = request.Name;
        if (request.Status.HasValue) adGroup.Status = request.Status.Value;
        if (request.BiddingStrategy.HasValue) adGroup.BiddingStrategy = request.BiddingStrategy.Value;
        if (request.BidAmount.HasValue) adGroup.BidAmount = request.BidAmount.Value;
        if (request.TargetingRules != null) adGroup.TargetingRules = request.TargetingRules;

        adGroup.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var detail = new AdGroupDetailDto(
            adGroup.Id,
            adGroup.Name,
            adGroup.Status,
            adGroup.BiddingStrategy,
            adGroup.BidAmount,
            adGroup.TargetingRules,
            adGroup.CreatedAt,
            adGroup.UpdatedAt,
            adGroup.Creatives.Select(c => new CreativeListDto(
                c.Id,
                c.Name,
                c.Type,
                c.Status,
                c.Headline,
                c.ImageUrl,
                c.CreatedAt
            )).ToList()
        );

        return Ok(detail);
    }

    /// <summary>
    /// Delete an ad group
    /// </summary>
    [HttpDelete("api/adgroups/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAdGroup(Guid id)
    {
        var tenantId = GetTenantId();
        var adGroup = await _context.AdGroups
            .FirstOrDefaultAsync(ag => ag.Id == id && ag.TenantId == tenantId);

        if (adGroup == null)
        {
            return NotFound(new { message = "Ad group not found" });
        }

        _context.AdGroups.Remove(adGroup);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
