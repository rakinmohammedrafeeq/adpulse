using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;

namespace AdPulse.API.Controllers;

[ApiController]
[Authorize]
public class CreativesController : ControllerBase
{
    private readonly AdPulseDbContext _context;
    private readonly ILogger<CreativesController> _logger;

    public CreativesController(AdPulseDbContext context, ILogger<CreativesController> logger)
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
    /// Get all creatives for an ad group
    /// </summary>
    [HttpGet("api/adgroups/{adGroupId}/creatives")]
    [ProducesResponseType(typeof(List<CreativeListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCreatives(Guid adGroupId)
    {
        var tenantId = GetTenantId();
        var creatives = await _context.Creatives
            .Where(c => c.AdGroupId == adGroupId && c.TenantId == tenantId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CreativeListDto(
                c.Id,
                c.Name,
                c.Type,
                c.Status,
                c.Headline,
                c.ImageUrl,
                c.CreatedAt
            ))
            .ToListAsync();

        return Ok(creatives);
    }

    /// <summary>
    /// Get specific creative details
    /// </summary>
    [HttpGet("api/creatives/{id}")]
    [ProducesResponseType(typeof(CreativeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCreative(Guid id)
    {
        var tenantId = GetTenantId();
        var creative = await _context.Creatives
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (creative == null)
        {
            return NotFound(new { message = "Creative not found" });
        }

        var detail = new CreativeDetailDto(
            creative.Id,
            creative.Name,
            creative.Type,
            creative.Status,
            creative.Headline,
            creative.Description,
            creative.ImageUrl,
            creative.VideoUrl,
            creative.DestinationUrl,
            creative.CallToAction,
            creative.Width,
            creative.Height,
            creative.CreatedAt,
            creative.UpdatedAt
        );

        return Ok(detail);
    }

    /// <summary>
    /// Create a creative within an ad group
    /// </summary>
    [HttpPost("api/adgroups/{adGroupId}/creatives")]
    [ProducesResponseType(typeof(CreativeDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCreative(Guid adGroupId, [FromBody] CreateCreativeRequest request)
    {
        var tenantId = GetTenantId();

        // Verify ad group belongs to tenant
        var adGroupExists = await _context.AdGroups.AnyAsync(ag => ag.Id == adGroupId && ag.TenantId == tenantId);
        if (!adGroupExists)
        {
            return NotFound(new { message = "Ad group not found" });
        }

        var creative = new Creative
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AdGroupId = adGroupId,
            Name = request.Name,
            Type = request.Type,
            Status = CreativeStatus.Active,
            Headline = request.Headline,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            VideoUrl = request.VideoUrl,
            DestinationUrl = request.DestinationUrl,
            CallToAction = request.CallToAction,
            Width = request.Width,
            Height = request.Height,
            CreatedAt = DateTime.UtcNow
        };

        _context.Creatives.Add(creative);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Creative created: {CreativeId} for ad group {AdGroupId}", creative.Id, adGroupId);

        var detail = new CreativeDetailDto(
            creative.Id,
            creative.Name,
            creative.Type,
            creative.Status,
            creative.Headline,
            creative.Description,
            creative.ImageUrl,
            creative.VideoUrl,
            creative.DestinationUrl,
            creative.CallToAction,
            creative.Width,
            creative.Height,
            creative.CreatedAt,
            creative.UpdatedAt
        );

        return CreatedAtAction(nameof(GetCreative), new { id = creative.Id }, detail);
    }

    /// <summary>
    /// Update an existing creative
    /// </summary>
    [HttpPut("api/creatives/{id}")]
    [ProducesResponseType(typeof(CreativeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCreative(Guid id, [FromBody] UpdateCreativeRequest request)
    {
        var tenantId = GetTenantId();
        var creative = await _context.Creatives
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (creative == null)
        {
            return NotFound(new { message = "Creative not found" });
        }

        if (request.Name != null) creative.Name = request.Name;
        if (request.Status.HasValue) creative.Status = request.Status.Value;
        if (request.Headline != null) creative.Headline = request.Headline;
        if (request.Description != null) creative.Description = request.Description;
        if (request.ImageUrl != null) creative.ImageUrl = request.ImageUrl;
        if (request.VideoUrl != null) creative.VideoUrl = request.VideoUrl;
        if (request.DestinationUrl != null) creative.DestinationUrl = request.DestinationUrl;
        if (request.CallToAction != null) creative.CallToAction = request.CallToAction;

        creative.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var detail = new CreativeDetailDto(
            creative.Id,
            creative.Name,
            creative.Type,
            creative.Status,
            creative.Headline,
            creative.Description,
            creative.ImageUrl,
            creative.VideoUrl,
            creative.DestinationUrl,
            creative.CallToAction,
            creative.Width,
            creative.Height,
            creative.CreatedAt,
            creative.UpdatedAt
        );

        return Ok(detail);
    }

    /// <summary>
    /// Delete a creative
    /// </summary>
    [HttpDelete("api/creatives/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCreative(Guid id)
    {
        var tenantId = GetTenantId();
        var creative = await _context.Creatives
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (creative == null)
        {
            return NotFound(new { message = "Creative not found" });
        }

        _context.Creatives.Remove(creative);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
