using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services;
using AdPulse.API.Services.Elasticsearch;
using Xunit;

namespace AdPulse.Tests.Unit;

public class TenantIsolationTests
{
    private readonly Guid _tenantA = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _tenantB = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly Guid _userA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private readonly Guid _userB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private AdPulseDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AdPulseDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AdPulseDbContext(options);
    }

    private async Task SeedMultiTenantDataAsync(AdPulseDbContext context)
    {
        // Add Tenant A
        var tenantA = new Tenant
        {
            Id = _tenantA,
            Name = "Tenant Alpha",
            CompanyName = "Alpha Corp",
            IsActive = true
        };

        // Add Tenant B
        var tenantB = new Tenant
        {
            Id = _tenantB,
            Name = "Tenant Beta",
            CompanyName = "Beta Corp",
            IsActive = true
        };

        context.Tenants.AddRange(tenantA, tenantB);

        // Add Campaign for Tenant A
        var campaignA = new Campaign
        {
            Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
            TenantId = _tenantA,
            Name = "Alpha Campaign 1",
            Objective = CampaignObjective.Conversions,
            DailyBudget = 100m,
            StartDate = DateTime.UtcNow,
            CreatedBy = _userA
        };

        // Add Campaign for Tenant B
        var campaignB = new Campaign
        {
            Id = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
            TenantId = _tenantB,
            Name = "Beta Campaign 1",
            Objective = CampaignObjective.BrandAwareness,
            DailyBudget = 200m,
            StartDate = DateTime.UtcNow,
            CreatedBy = _userB
        };

        context.Campaigns.AddRange(campaignA, campaignB);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetCampaignsAsync_ReturnsOnlyCurrentTenantData()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);
        await SeedMultiTenantDataAsync(context);

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var campaignService = new CampaignService(context, mockEs.Object, mockLogger.Object);

        // Act - Request as Tenant A
        var campaignsForA = await campaignService.GetCampaignsAsync(_tenantA);

        // Assert
        Assert.Single(campaignsForA);
        Assert.Equal("Alpha Campaign 1", campaignsForA[0].Name);
        Assert.DoesNotContain(campaignsForA, c => c.Name.Contains("Beta"));
    }

    [Fact]
    public async Task GetCampaignByIdAsync_TenantCannotAccessOtherTenantCampaign()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);
        await SeedMultiTenantDataAsync(context);

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var campaignService = new CampaignService(context, mockEs.Object, mockLogger.Object);

        var betaCampaignId = Guid.Parse("b0000000-0000-0000-0000-000000000001");

        // Act - Tenant A attempts to access Tenant B's campaign by ID
        var result = await campaignService.GetCampaignByIdAsync(betaCampaignId, _tenantA);

        // Assert - Must return null
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCampaignAsync_TenantCannotModifyOtherTenantCampaign()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);
        await SeedMultiTenantDataAsync(context);

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var campaignService = new CampaignService(context, mockEs.Object, mockLogger.Object);

        var betaCampaignId = Guid.Parse("b0000000-0000-0000-0000-000000000001");
        var updateRequest = new UpdateCampaignRequest(
            Name: "Hacked Beta Campaign",
            Description: null,
            Status: null,
            DailyBudget: 9999m,
            TotalBudget: null,
            StartDate: null,
            EndDate: null
        );

        // Act - Tenant A attempts to update Tenant B's campaign
        var updateResult = await campaignService.UpdateCampaignAsync(betaCampaignId, updateRequest, _tenantA);

        // Assert
        Assert.Null(updateResult);

        // Verify original data in database was NOT changed
        var betaCampaignInDb = await context.Campaigns.IgnoreQueryFilters().FirstAsync(c => c.Id == betaCampaignId);
        Assert.Equal("Beta Campaign 1", betaCampaignInDb.Name);
        Assert.Equal(200m, betaCampaignInDb.DailyBudget);
    }

    [Fact]
    public async Task DeleteCampaignAsync_TenantCannotDeleteOtherTenantCampaign()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);
        await SeedMultiTenantDataAsync(context);

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var campaignService = new CampaignService(context, mockEs.Object, mockLogger.Object);

        var betaCampaignId = Guid.Parse("b0000000-0000-0000-0000-000000000001");

        // Act - Tenant A attempts to delete Tenant B's campaign
        var deleteResult = await campaignService.DeleteCampaignAsync(betaCampaignId, _tenantA);

        // Assert
        Assert.False(deleteResult);

        // Verify campaign still exists in database
        var exists = await context.Campaigns.IgnoreQueryFilters().AnyAsync(c => c.Id == betaCampaignId);
        Assert.True(exists);
    }

    [Fact]
    public async Task GlobalQueryFilter_AutomaticallyFiltersByTenant()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);
        await SeedMultiTenantDataAsync(context);

        // Act - Set current tenant context to Tenant A
        context.SetCurrentTenant(_tenantA);
        var visibleCampaigns = await context.Campaigns.ToListAsync();

        // Assert
        Assert.Single(visibleCampaigns);
        Assert.Equal(_tenantA, visibleCampaigns[0].TenantId);
        Assert.Equal("Alpha Campaign 1", visibleCampaigns[0].Name);
    }
}
