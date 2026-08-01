using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AdPulse.API.Controllers;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services.Elasticsearch;
using System.Security.Claims;
using Xunit;

namespace AdPulse.Tests.Unit;

public class EventsControllerTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _campaignId = Guid.NewGuid();

    private AdPulseDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AdPulseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AdPulseDbContext(options);
    }

    [Fact]
    public async Task IngestEvent_ValidEvent_SavesToDbAndIndexesToElasticsearch()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        // Seed Campaign
        var campaign = new Campaign
        {
            Id = _campaignId,
            TenantId = _tenantId,
            Name = "Active Campaign",
            Objective = CampaignObjective.Conversions,
            DailyBudget = 100m,
            StartDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync();

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<EventsController>>();
        var controller = new EventsController(context, mockEs.Object, mockLogger.Object);

        // Setup HttpContext
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Tenant-ID"] = _tenantId.ToString();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var request = new IngestEventRequest(
            CampaignId: _campaignId,
            AdGroupId: null,
            CreativeId: null,
            EventType: AdEventType.Click,
            EventTime: DateTime.UtcNow,
            UserId: "usr_42",
            SessionId: "sess_1",
            IpAddress: "127.0.0.1",
            UserAgent: "Mozilla/5.0",
            DeviceType: "Desktop",
            Country: "United States",
            City: "Seattle",
            Referrer: "https://example.com",
            ConversionValue: null,
            CustomData: null
        );

        // Act
        var result = await controller.IngestEvent(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var eventDto = Assert.IsType<EventDto>(createdResult.Value);
        Assert.Equal(AdEventType.Click, eventDto.EventType);
        Assert.Equal(_campaignId, eventDto.CampaignId);

        // Verify entity in DbContext
        var inDb = await context.AdEvents.FirstOrDefaultAsync(e => e.Id == eventDto.Id);
        Assert.NotNull(inDb);
        Assert.Equal("usr_42", inDb.UserId);

        // Verify Elasticsearch indexing was called
        mockEs.Verify(es => es.IndexEventAsync(It.Is<AdEvent>(e => e.Id == eventDto.Id)), Times.Once);
    }

    [Fact]
    public async Task IngestBatch_MultipleEvents_SavesAllEvents()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var campaign = new Campaign
        {
            Id = _campaignId,
            TenantId = _tenantId,
            Name = "Batch Campaign",
            Objective = CampaignObjective.Traffic,
            DailyBudget = 200m,
            StartDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid()
        };
        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync();

        var mockEs = new Mock<IElasticsearchService>();
        var mockLogger = new Mock<ILogger<EventsController>>();
        var controller = new EventsController(context, mockEs.Object, mockLogger.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Tenant-ID"] = _tenantId.ToString();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        var batch = new BatchIngestRequest(new List<IngestEventRequest>
        {
            new(_campaignId, null, null, AdEventType.Impression, DateTime.UtcNow, "u1", "s1", "127.0.0.1", "agent", "Desktop", "US", "NY", null, null, null),
            new(_campaignId, null, null, AdEventType.Click, DateTime.UtcNow, "u1", "s1", "127.0.0.1", "agent", "Desktop", "US", "NY", null, null, null),
            new(_campaignId, null, null, AdEventType.Conversion, DateTime.UtcNow, "u1", "s1", "127.0.0.1", "agent", "Desktop", "US", "NY", null, 150m, null)
        });

        // Act
        var result = await controller.IngestBatch(batch);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<BatchIngestResponse>(okResult.Value);
        Assert.Equal(3, response.Successful);
        Assert.Equal(0, response.Failed);

        var count = await context.AdEvents.CountAsync();
        Assert.Equal(3, count);
    }
}
