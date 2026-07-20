using System.ComponentModel.DataAnnotations;
using AdPulse.API.Models;

namespace AdPulse.API.DTOs;

public record AudienceDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    AudienceType Type,
    string? Demographics,
    string? Interests,
    string? Behaviors,
    string? Locations,
    string? Devices,
    int EstimatedSize,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateAudienceRequest(
    [Required] string Name,
    string? Description,
    [Required] AudienceType Type,
    string? Demographics,
    string? Interests,
    string? Behaviors,
    string? Locations,
    string? Devices,
    int EstimatedSize
);

public record UpdateAudienceRequest(
    string? Name,
    string? Description,
    AudienceType? Type,
    string? Demographics,
    string? Interests,
    string? Behaviors,
    string? Locations,
    string? Devices,
    int? EstimatedSize
);
