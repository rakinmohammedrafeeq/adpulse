namespace AdPulse.API.Models;

/// <summary>
/// Represents a target audience segment
/// </summary>
public class Audience
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AudienceType Type { get; set; }
    
    // Targeting criteria (JSON serialized)
    public string? Demographics { get; set; }
    public string? Interests { get; set; }
    public string? Behaviors { get; set; }
    public string? Locations { get; set; }
    public string? Devices { get; set; }
    
    public int EstimatedSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<AdGroup> AdGroups { get; set; } = new List<AdGroup>();
}

public enum AudienceType
{
    Custom,
    Lookalike,
    Retargeting,
    Demographics,
    Interests,
    InMarket
}
