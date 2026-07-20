namespace AdPulse.API.Models;

/// <summary>
/// Represents an advertising campaign
/// </summary>
public class Campaign
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;
    public CampaignObjective Objective { get; set; }
    
    // Budget and Scheduling
    public decimal DailyBudget { get; set; }
    public decimal? TotalBudget { get; set; }
    public decimal SpentAmount { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<AdGroup> AdGroups { get; set; } = new List<AdGroup>();
}

public enum CampaignStatus
{
    Draft,
    Scheduled,
    Active,
    Paused,
    Completed,
    Archived
}

public enum CampaignObjective
{
    BrandAwareness,
    Reach,
    Traffic,
    Engagement,
    Conversions,
    AppInstalls,
    VideoViews,
    LeadGeneration
}
