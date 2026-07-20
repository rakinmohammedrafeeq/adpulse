namespace AdPulse.API.Models;

/// <summary>
/// Represents an ad event (impression, click, conversion)
/// </summary>
public class AdEvent
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CampaignId { get; set; }
    public Guid? AdGroupId { get; set; }
    public Guid? CreativeId { get; set; }
    
    public AdEventType EventType { get; set; }
    public DateTime EventTime { get; set; } = DateTime.UtcNow;
    
    // User context
    public string? UserId { get; set; }
    public string? SessionId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceType { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    
    // Event metadata
    public string? Referrer { get; set; }
    public decimal? ConversionValue { get; set; }
    public string? CustomData { get; set; } // JSON
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Campaign Campaign { get; set; } = null!;
}

public enum AdEventType
{
    Impression,
    Click,
    Conversion,
    VideoView,
    VideoComplete,
    AppInstall
}
