using System.ComponentModel.DataAnnotations;
using AdPulse.API.Models;

namespace AdPulse.API.DTOs;

public record IngestEventRequest(
    [Required] Guid CampaignId,
    Guid? AdGroupId,
    Guid? CreativeId,
    [Required] AdEventType EventType,
    DateTime? EventTime,
    string? UserId,
    string? SessionId,
    string? IpAddress,
    string? UserAgent,
    string? DeviceType,
    string? Country,
    string? City,
    string? Referrer,
    decimal? ConversionValue,
    string? CustomData
);

public record EventDto(
    Guid Id,
    Guid TenantId,
    Guid CampaignId,
    Guid? AdGroupId,
    Guid? CreativeId,
    AdEventType EventType,
    DateTime EventTime,
    string? UserId,
    string? DeviceType,
    string? Country,
    string? City,
    decimal? ConversionValue
);

public record BatchIngestRequest(
    [Required] List<IngestEventRequest> Events
);

public record BatchIngestResponse(
    int Successful,
    int Failed,
    List<string> Errors
);
