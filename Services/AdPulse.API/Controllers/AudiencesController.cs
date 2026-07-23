using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;

namespace AdPulse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AudiencesController : ControllerBase
{
    private readonly AdPulseDbContext _context;
    private readonly ILogger<AudiencesController> _logger;

    public AudiencesController(AdPulseDbContext context, ILogger<AudiencesController> logger)
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
    /// Get all audiences for the current tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<AudienceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAudiences()
    {
        var tenantId = GetTenantId();
        var audiences = await _context.Audiences
            .Where(a => a.TenantId == tenantId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AudienceDto(
                a.Id,
                a.TenantId,
                a.Name,
                a.Description,
                a.Type,
                a.Demographics,
                a.Interests,
                a.Behaviors,
                a.Locations,
                a.Devices,
                a.EstimatedSize,
                a.CreatedAt,
                a.UpdatedAt
            ))
            .ToListAsync();

        return Ok(audiences);
    }

    /// <summary>
    /// Get audience by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AudienceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAudience(Guid id)
    {
        var tenantId = GetTenantId();
        var audience = await _context.Audiences
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

        if (audience == null)
        {
            return NotFound(new { message = "Audience not found" });
        }

        var dto = new AudienceDto(
            audience.Id,
            audience.TenantId,
            audience.Name,
            audience.Description,
            audience.Type,
            audience.Demographics,
            audience.Interests,
            audience.Behaviors,
            audience.Locations,
            audience.Devices,
            audience.EstimatedSize,
            audience.CreatedAt,
            audience.UpdatedAt
        );

        return Ok(dto);
    }

    /// <summary>
    /// Create a new audience segment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AudienceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAudience([FromBody] CreateAudienceRequest request)
    {
        var tenantId = GetTenantId();

        var audience = new Audience
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            Demographics = request.Demographics,
            Interests = request.Interests,
            Behaviors = request.Behaviors,
            Locations = request.Locations,
            Devices = request.Devices,
            EstimatedSize = request.EstimatedSize,
            CreatedAt = DateTime.UtcNow
        };

        _context.Audiences.Add(audience);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Audience created: {AudienceId} for tenant {TenantId}", audience.Id, tenantId);

        var dto = new AudienceDto(
            audience.Id,
            audience.TenantId,
            audience.Name,
            audience.Description,
            audience.Type,
            audience.Demographics,
            audience.Interests,
            audience.Behaviors,
            audience.Locations,
            audience.Devices,
            audience.EstimatedSize,
            audience.CreatedAt,
            audience.UpdatedAt
        );

        return CreatedAtAction(nameof(GetAudience), new { id = audience.Id }, dto);
    }

    /// <summary>
    /// Update an existing audience
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AudienceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAudience(Guid id, [FromBody] UpdateAudienceRequest request)
    {
        var tenantId = GetTenantId();
        var audience = await _context.Audiences
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

        if (audience == null)
        {
            return NotFound(new { message = "Audience not found" });
        }

        if (request.Name != null) audience.Name = request.Name;
        if (request.Description != null) audience.Description = request.Description;
        if (request.Type.HasValue) audience.Type = request.Type.Value;
        if (request.Demographics != null) audience.Demographics = request.Demographics;
        if (request.Interests != null) audience.Interests = request.Interests;
        if (request.Behaviors != null) audience.Behaviors = request.Behaviors;
        if (request.Locations != null) audience.Locations = request.Locations;
        if (request.Devices != null) audience.Devices = request.Devices;
        if (request.EstimatedSize.HasValue) audience.EstimatedSize = request.EstimatedSize.Value;

        audience.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var dto = new AudienceDto(
            audience.Id,
            audience.TenantId,
            audience.Name,
            audience.Description,
            audience.Type,
            audience.Demographics,
            audience.Interests,
            audience.Behaviors,
            audience.Locations,
            audience.Devices,
            audience.EstimatedSize,
            audience.CreatedAt,
            audience.UpdatedAt
        );

        return Ok(dto);
    }

    /// <summary>
    /// Delete an audience
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAudience(Guid id)
    {
        var tenantId = GetTenantId();
        var audience = await _context.Audiences
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

        if (audience == null)
        {
            return NotFound(new { message = "Audience not found" });
        }

        _context.Audiences.Remove(audience);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
