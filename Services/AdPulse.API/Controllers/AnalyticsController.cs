using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdPulse.API.DTOs;
using AdPulse.API.Services;
using System.Security.Claims;

namespace AdPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    private Guid GetTenantId()
    {
        return Guid.Parse(User.FindFirst("tenant_id")?.Value 
            ?? throw new UnauthorizedAccessException("Tenant ID not found"));
    }

    /// <summary>
    /// Get dashboard statistics for the current tenant
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats()
    {
        var tenantId = GetTenantId();
        var stats = await _analyticsService.GetDashboardStatsAsync(tenantId);
        return Ok(stats);
    }

    /// <summary>
    /// Get analytics for a specific campaign
    /// </summary>
    [HttpGet("campaigns/{campaignId}")]
    [ProducesResponseType(typeof(CampaignAnalyticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignAnalytics(
        Guid campaignId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var tenantId = GetTenantId();
        
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        try
        {
            var analytics = await _analyticsService.GetCampaignAnalyticsAsync(campaignId, tenantId, start, end);
            return Ok(analytics);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get time-series data for a campaign
    /// </summary>
    [HttpGet("campaigns/{campaignId}/timeseries")]
    [ProducesResponseType(typeof(List<TimeSeriesDataPoint>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaignTimeSeries(
        Guid campaignId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var tenantId = GetTenantId();
        
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var timeSeries = await _analyticsService.GetCampaignTimeSeriesAsync(campaignId, tenantId, start, end);
        return Ok(timeSeries);
    }
}
