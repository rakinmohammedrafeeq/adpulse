namespace AdPulse.API.Models;

/// <summary>
/// Represents a tenant (advertiser organization) in the multi-tenant system
/// </summary>
public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
}
