using Microsoft.EntityFrameworkCore;
using AdPulse.API.Models;

namespace AdPulse.API.Data;

public class AdPulseDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private Guid? _currentTenantId;

    public AdPulseDbContext(DbContextOptions<AdPulseDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<AdGroup> AdGroups => Set<AdGroup>();
    public DbSet<Creative> Creatives => Set<Creative>();
    public DbSet<Audience> Audiences => Set<Audience>();
    public DbSet<AdEvent> AdEvents => Set<AdEvent>();

    /// <summary>
    /// Set the current tenant ID for query filtering
    /// </summary>
    public void SetCurrentTenant(Guid tenantId)
    {
        _currentTenantId = tenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Industry).HasMaxLength(100);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Email, e.TenantId }).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });

        // Campaign
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Name });
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DailyBudget).HasPrecision(18, 2);
            entity.Property(e => e.TotalBudget).HasPrecision(18, 2);
            entity.Property(e => e.SpentAmount).HasPrecision(18, 2);

            entity.HasOne(e => e.Tenant)
                .WithMany(t => t.Campaigns)
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });

        // AdGroup
        modelBuilder.Entity<AdGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.CampaignId });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BidAmount).HasPrecision(18, 2);
            entity.Property(e => e.TargetingRules).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Campaign)
                .WithMany(c => c.AdGroups)
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });

        // Creative
        modelBuilder.Entity<Creative>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.AdGroupId });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Headline).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.VideoUrl).HasMaxLength(1000);
            entity.Property(e => e.DestinationUrl).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CallToAction).HasMaxLength(100);

            entity.HasOne(e => e.AdGroup)
                .WithMany(ag => ag.Creatives)
                .HasForeignKey(e => e.AdGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });

        // Audience
        modelBuilder.Entity<Audience>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Name });
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Demographics).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Interests).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Behaviors).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Locations).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Devices).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many relationship with AdGroups
            entity.HasMany(e => e.AdGroups)
                .WithMany(ag => ag.Audiences)
                .UsingEntity<Dictionary<string, object>>(
                    "AdGroupAudience",
                    j => j.HasOne<AdGroup>().WithMany().HasForeignKey("AdGroupId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Audience>().WithMany().HasForeignKey("AudienceId").OnDelete(DeleteBehavior.NoAction)
                );

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });

        // AdEvent
        modelBuilder.Entity<AdEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.CampaignId, e.EventTime });
            entity.HasIndex(e => new { e.TenantId, e.EventType, e.EventTime });
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.SessionId).HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Referrer).HasMaxLength(2000);
            entity.Property(e => e.ConversionValue).HasPrecision(18, 2);
            entity.Property(e => e.CustomData).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Campaign)
                .WithMany()
                .HasForeignKey(e => e.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            // Global query filter for multi-tenancy
            entity.HasQueryFilter(e => _currentTenantId == null || e.TenantId == _currentTenantId);
        });
    }

    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
