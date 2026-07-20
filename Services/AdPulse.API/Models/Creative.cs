namespace AdPulse.API.Models;

/// <summary>
/// Represents an ad creative (the actual ad content)
/// </summary>
public class Creative
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid AdGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CreativeType Type { get; set; }
    public CreativeStatus Status { get; set; } = CreativeStatus.Active;
    
    // Content
    public string Headline { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string DestinationUrl { get; set; } = string.Empty;
    public string? CallToAction { get; set; }
    
    // Dimensions (for display ads)
    public int? Width { get; set; }
    public int? Height { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public AdGroup AdGroup { get; set; } = null!;
}

public enum CreativeType
{
    Image,
    Video,
    Carousel,
    Native,
    Text,
    ResponsiveDisplay
}

public enum CreativeStatus
{
    Active,
    Paused,
    Rejected,
    Archived
}
