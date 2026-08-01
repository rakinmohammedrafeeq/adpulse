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

public class CampaignServiceTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private AdPulseDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AdPulseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AdPulseDbContext(options);
    }

    [Fact]
    public async Task CreateCampaignAsync_PersistsToDatabase_AndIndexesToElasticsearch()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var service = new CampaignService(context, mockEs.Object, mockLogger.Object);

        var request = new CreateCampaignRequest(
            Name: "New Product Launch",
            Description: "Acquisition campaign",
            Objective: CampaignObjective.Traffic,
            DailyBudget: 350m,
            TotalBudget: 7000m,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(30)
        );

        // Act
        var result = await service.CreateCampaignAsync(request, _tenantId, _userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product Launch", result.Name);
        Assert.Equal(350m, result.DailyBudget);
        Assert.Equal(CampaignStatus.Draft, result.Status);

        // Verify entity in DbContext
        var savedCampaign = await context.Campaigns.FirstOrDefaultAsync(c => c.Id == result.Id);
        Assert.NotNull(savedCampaign);
        Assert.Equal(_tenantId, savedCampaign.TenantId);
        Assert.Equal(_userId, savedCampaign.CreatedBy);

        // Verify Elasticsearch indexing was called
        mockEs.Verify(es => es.IndexCampaignAsync(It.Is<Campaign>(c => c.Id == result.Id)), Times.Once);
    }

    [Fact]
    public async Task UpdateCampaignAsync_ModifiesFields_AndCallsElasticsearchUpdate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var campaignId = Guid.NewGuid();
        var existing = new Campaign
        {
            Id = campaignId,
            TenantId = _tenantId,
            Name = "Initial Name",
            DailyBudget = 100m,
            Status = CampaignStatus.Draft,
            Objective = CampaignObjective.Conversions,
            StartDate = DateTime.UtcNow,
            CreatedBy = _userId
        };
        context.Campaigns.Add(existing);
        await context.SaveChangesAsync();

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var service = new CampaignService(context, mockEs.Object, mockLogger.Object);

        var updateRequest = new UpdateCampaignRequest(
            Name: "Updated Name",
            Description: "Updated Description",
            Status: CampaignStatus.Active,
            DailyBudget: 250m,
            TotalBudget: 5000m,
            StartDate: null,
            EndDate: null
        );

        // Act
        var result = await service.UpdateCampaignAsync(campaignId, updateRequest, _tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(CampaignStatus.Active, result.Status);
        Assert.Equal(250m, result.DailyBudget);

        // Verify database persistence
        var updatedInDb = await context.Campaigns.FirstAsync(c => c.Id == campaignId);
        Assert.Equal("Updated Name", updatedInDb.Name);
        Assert.Equal(CampaignStatus.Active, updatedInDb.Status);

        // Verify Elasticsearch update was invoked
        mockEs.Verify(es => es.UpdateCampaignAsync(It.Is<Campaign>(c => c.Id == campaignId)), Times.Once);
    }

    [Fact]
    public async Task DeleteCampaignAsync_RemovesCampaign_AndCallsElasticsearchDelete()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var campaignId = Guid.NewGuid();
        var campaign = new Campaign
        {
            Id = campaignId,
            TenantId = _tenantId,
            Name = "To Delete",
            DailyBudget = 50m,
            StartDate = DateTime.UtcNow,
            CreatedBy = _userId
        };
        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync();

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<CampaignService>>();
        var service = new CampaignService(context, mockEs.Object, mockLogger.Object);

        // Act
        var success = await service.DeleteCampaignAsync(campaignId, _tenantId);

        // Assert
        Assert.True(success);
        var inDb = await context.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId);
        Assert.Null(inDb);

        // Verify Elasticsearch delete
        mockEs.Verify(es => es.DeleteCampaignAsync(campaignId), Times.Once);
    }
}
