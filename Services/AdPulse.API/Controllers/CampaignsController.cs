using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdPulse.API.DTOs;
using AdPulse.API.Services;
using System.Security.Claims;

namespace AdPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignsController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(ICampaignService campaignService, ILogger<CampaignsController> logger)
    {
        _campaignService = campaignService;
        _logger = logger;
    }

    private Guid GetTenantId()
    {
        return Guid.Parse(User.FindFirst("tenant_id")?.Value 
            ?? throw new UnauthorizedAccessException("Tenant ID not found"));
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    /// <summary>
    /// Get all campaigns for the current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CampaignListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCampaigns()
    {
        var tenantId = GetTenantId();
        var campaigns = await _campaignService.GetCampaignsAsync(tenantId);
        return Ok(campaigns);
    }

    /// <summary>
    /// Get a specific campaign by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CampaignDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaign(Guid id)
    {
        var tenantId = GetTenantId();
        var campaign = await _campaignService.GetCampaignByIdAsync(id, tenantId);

        if (campaign == null)
        {
            return NotFound(new { message = "Campaign not found" });
        }

        return Ok(campaign);
    }

    /// <summary>
    /// Create a new campaign
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CampaignDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();

        var campaign = await _campaignService.CreateCampaignAsync(request, tenantId, userId);

        return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
    }

    /// <summary>
    /// Update an existing campaign
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CampaignDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignRequest request)
    {
        var tenantId = GetTenantId();
        var campaign = await _campaignService.UpdateCampaignAsync(id, request, tenantId);

        if (campaign == null)
        {
            return NotFound(new { message = "Campaign not found" });
        }

        return Ok(campaign);
    }

    /// <summary>
    /// Delete a campaign
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCampaign(Guid id)
    {
        var tenantId = GetTenantId();
        var success = await _campaignService.DeleteCampaignAsync(id, tenantId);

        if (!success)
        {
            return NotFound(new { message = "Campaign not found" });
        }

        return NoContent();
    }
}
