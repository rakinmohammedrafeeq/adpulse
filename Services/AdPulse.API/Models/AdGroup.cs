namespace AdPulse.API.Models;

/// <summary>
/// Represents an ad group within a campaign
/// </summary>
public class AdGroup
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public AdGroupStatus Status { get; set; } = AdGroupStatus.Active;
    
    // Bidding
    public BiddingStrategy BiddingStrategy { get; set; }
    public decimal BidAmount { get; set; }
    
    // Targeting
    public string? TargetingRules { get; set; } // JSON serialized targeting criteria
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Campaign Campaign { get; set; } = null!;
    public ICollection<Creative> Creatives { get; set; } = new List<Creative>();
    public ICollection<Audience> Audiences { get; set; } = new List<Audience>();
}

public enum AdGroupStatus
{
    Active,
    Paused,
    Archived
}

public enum BiddingStrategy
{
    ManualCPC,
    ManualCPM,
    AutomaticCPC,
    TargetCPA,
    TargetROAS,
    MaximizeClicks,
    MaximizeConversions
}
