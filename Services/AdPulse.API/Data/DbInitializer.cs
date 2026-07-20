using Microsoft.EntityFrameworkCore;
using AdPulse.API.Models;
using AdPulse.API.Services.Elasticsearch;

namespace AdPulse.API.Data;

public static class DbInitializer
{
    public static readonly Guid DemoTenantId = Guid.Parse("e7b1a2c3-d4e5-4f6a-8b9c-0d1e2f3a4b5c");
    public static readonly Guid DemoUserId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-1e2f3a4b5c6d");
    public static readonly Guid DemoCampaign1Id = Guid.Parse("c1d2e3f4-a5b6-4c7d-8e9f-0a1b2c3d4e5f");
    public static readonly Guid DemoCampaign2Id = Guid.Parse("c2d3e4f5-a6b7-4c8d-9e0f-1a2b3c4d5e6f");

    public static async Task InitializeAsync(
        AdPulseDbContext context,
        IElasticsearchService elasticsearchService,
        ILogger logger)
    {
        try
        {
            logger.LogInformation("Checking database schema and ensuring tables exist...");
            await context.Database.EnsureCreatedAsync();

            // Disable query filters for seeding check
            var hasTenants = await context.Tenants.IgnoreQueryFilters().AnyAsync();
            if (!hasTenants)
            {
                logger.LogInformation("Seeding initial demo data for tenant Acme Corporation...");

                // 1. Seed Tenant
                var tenant = new Tenant
                {
                    Id = DemoTenantId,
                    Name = "Acme Corporation",
                    CompanyName = "Acme Corp Global LLC",
                    Website = "https://acme.example.com",
                    Industry = "Cloud & AdTech",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };
                context.Tenants.Add(tenant);

                // 2. Seed User
                var user = new User
                {
                    Id = DemoUserId,
                    TenantId = DemoTenantId,
                    Email = "admin@acme.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    FirstName = "Alex",
                    LastName = "Administrator",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                };
                context.Users.Add(user);

                // 3. Seed Audiences
                var audience1 = new Audience
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    Name = "B2B Enterprise Decision Makers",
                    Description = "VP and C-level tech executives in North America and EMEA",
                    Type = AudienceType.Custom,
                    Demographics = "{\"ageRange\": \"30-65\", \"jobSeniority\": \"Director,VP,CXO\"}",
                    Interests = "[\"Cloud Computing\", \"Marketing Automation\", \"Enterprise SaaS\"]",
                    Locations = "[\"United States\", \"United Kingdom\", \"Germany\", \"Canada\"]",
                    Devices = "[\"Desktop\", \"Mobile\"]",
                    EstimatedSize = 850000,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                };

                var audience2 = new Audience
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    Name = "High-Growth Software Developers",
                    Description = "Full stack developers and DevOps engineers looking for tooling",
                    Type = AudienceType.Lookalike,
                    Demographics = "{\"ageRange\": \"22-45\"}",
                    Interests = "[\"Kubernetes\", \"DevOps\", \"Web Development\", \"Cloud Architecture\"]",
                    Locations = "[\"Global\"]",
                    Devices = "[\"Desktop\"]",
                    EstimatedSize = 1400000,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                };

                context.Audiences.AddRange(audience1, audience2);

                // 4. Seed Campaigns
                var campaign1 = new Campaign
                {
                    Id = DemoCampaign1Id,
                    TenantId = DemoTenantId,
                    Name = "Q3 Global SaaS Expansion",
                    Description = "High-converting cross-platform advertising campaign for enterprise marketing teams",
                    Objective = CampaignObjective.Conversions,
                    Status = CampaignStatus.Active,
                    DailyBudget = 500.00m,
                    TotalBudget = 15000.00m,
                    SpentAmount = 3420.50m,
                    StartDate = DateTime.UtcNow.AddDays(-14),
                    EndDate = DateTime.UtcNow.AddDays(16),
                    CreatedBy = DemoUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-14)
                };

                var campaign2 = new Campaign
                {
                    Id = DemoCampaign2Id,
                    TenantId = DemoTenantId,
                    Name = "Brand Awareness - Video Series",
                    Description = "Video & rich media showcase demonstrating real-time ad performance",
                    Objective = CampaignObjective.BrandAwareness,
                    Status = CampaignStatus.Active,
                    DailyBudget = 250.00m,
                    TotalBudget = 7500.00m,
                    SpentAmount = 1850.00m,
                    StartDate = DateTime.UtcNow.AddDays(-7),
                    EndDate = DateTime.UtcNow.AddDays(23),
                    CreatedBy = DemoUserId,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                };

                context.Campaigns.AddRange(campaign1, campaign2);

                // 5. Seed AdGroups
                var adGroup1 = new AdGroup
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    CampaignId = DemoCampaign1Id,
                    Name = "Enterprise Decision Makers - Search & Display",
                    Status = AdGroupStatus.Active,
                    BiddingStrategy = BiddingStrategy.AutomaticCPC,
                    BidAmount = 2.45m,
                    TargetingRules = "{\"geo\": [\"US\", \"UK\"], \"device\": [\"Desktop\", \"Mobile\"]}",
                    CreatedAt = DateTime.UtcNow.AddDays(-14)
                };

                var adGroup2 = new AdGroup
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    CampaignId = DemoCampaign2Id,
                    Name = "Developer Community - In-Feed Video",
                    Status = AdGroupStatus.Active,
                    BiddingStrategy = BiddingStrategy.TargetCPA,
                    BidAmount = 1.85m,
                    TargetingRules = "{\"topics\": [\"Software\", \"Cloud\"]}",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                };

                context.AdGroups.AddRange(adGroup1, adGroup2);

                // 6. Seed Creatives
                var creative1 = new Creative
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    AdGroupId = adGroup1.Id,
                    Name = "AdPulse Platform Hero Banner",
                    Type = CreativeType.Image,
                    Status = CreativeStatus.Active,
                    Headline = "Transform Advertising Intelligence in Real-Time",
                    Description = "Harness predictive analytics and sub-millisecond bidding to scale campaign ROI.",
                    ImageUrl = "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=800&auto=format&fit=crop&q=60",
                    DestinationUrl = "https://adpulse.example.com/demo",
                    CallToAction = "Schedule Demo",
                    Width = 1200,
                    Height = 628,
                    CreatedAt = DateTime.UtcNow.AddDays(-14)
                };

                var creative2 = new Creative
                {
                    Id = Guid.NewGuid(),
                    TenantId = DemoTenantId,
                    AdGroupId = adGroup2.Id,
                    Name = "Dynamic Omnichannel Growth Showcase",
                    Type = CreativeType.ResponsiveDisplay,
                    Status = CreativeStatus.Active,
                    Headline = "Unleash Predictive Campaign Intelligence",
                    Description = "Maximize return on ad spend with sub-second multi-touch attribution and real-time audience optimization.",
                    ImageUrl = "https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=800&auto=format&fit=crop&q=60",
                    DestinationUrl = "https://adpulse.example.com/growth",
                    CallToAction = "Start Scaling",
                    Width = 600,
                    Height = 500,
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                };

                context.Creatives.AddRange(creative1, creative2);

                // 7. Seed Sample Ad Events (Past 7 days)
                var random = new Random(42);
                var adEvents = new List<AdEvent>();
                var deviceTypes = new[] { "Desktop", "Mobile", "Tablet" };
                var countries = new[] { "United States", "United Kingdom", "Germany", "Canada" };
                var cities = new[] { "New York", "London", "Berlin", "Toronto", "San Francisco" };

                for (int day = 7; day >= 0; day--)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-day);
                    int impressionsCount = random.Next(15, 30);

                    for (int i = 0; i < impressionsCount; i++)
                    {
                        var eventTime = date.AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
                        var isCampaign1 = random.NextDouble() > 0.4;
                        var campaignId = isCampaign1 ? DemoCampaign1Id : DemoCampaign2Id;
                        var adGroupId = isCampaign1 ? adGroup1.Id : adGroup2.Id;
                        var creativeId = isCampaign1 ? creative1.Id : creative2.Id;

                        // Impression
                        var impression = new AdEvent
                        {
                            Id = Guid.NewGuid(),
                            TenantId = DemoTenantId,
                            CampaignId = campaignId,
                            AdGroupId = adGroupId,
                            CreativeId = creativeId,
                            EventType = AdEventType.Impression,
                            EventTime = eventTime,
                            UserId = $"user_{random.Next(1000, 9999)}",
                            SessionId = Guid.NewGuid().ToString("N")[..12],
                            IpAddress = $"192.168.1.{random.Next(10, 250)}",
                            DeviceType = deviceTypes[random.Next(deviceTypes.Length)],
                            Country = countries[random.Next(countries.Length)],
                            City = cities[random.Next(cities.Length)]
                        };
                        adEvents.Add(impression);

                        // Click (~8-12% CTR)
                        if (random.NextDouble() < 0.12)
                        {
                            var click = new AdEvent
                            {
                                Id = Guid.NewGuid(),
                                TenantId = DemoTenantId,
                                CampaignId = campaignId,
                                AdGroupId = adGroupId,
                                CreativeId = creativeId,
                                EventType = AdEventType.Click,
                                EventTime = eventTime.AddSeconds(random.Next(2, 45)),
                                UserId = impression.UserId,
                                SessionId = impression.SessionId,
                                IpAddress = impression.IpAddress,
                                DeviceType = impression.DeviceType,
                                Country = impression.Country,
                                City = impression.City
                            };
                            adEvents.Add(click);

                            // Conversion (~20% of clicks)
                            if (random.NextDouble() < 0.20)
                            {
                                var convValue = (decimal)(random.Next(50, 450) + Math.Round(random.NextDouble(), 2));
                                var conversion = new AdEvent
                                {
                                    Id = Guid.NewGuid(),
                                    TenantId = DemoTenantId,
                                    CampaignId = campaignId,
                                    AdGroupId = adGroupId,
                                    CreativeId = creativeId,
                                    EventType = AdEventType.Conversion,
                                    EventTime = click.EventTime.AddSeconds(random.Next(30, 300)),
                                    UserId = impression.UserId,
                                    SessionId = impression.SessionId,
                                    IpAddress = impression.IpAddress,
                                    DeviceType = impression.DeviceType,
                                    Country = impression.Country,
                                    City = impression.City,
                                    ConversionValue = convValue
                                };
                                adEvents.Add(conversion);
                            }
                        }
                    }
                }

                context.AdEvents.AddRange(adEvents);
                await context.SaveChangesAsync();
                logger.LogInformation("Database seeded successfully with {EventsCount} sample events.", adEvents.Count);

                // Index campaigns in Elasticsearch
                try
                {
                    await elasticsearchService.IndexCampaignAsync(campaign1);
                    await elasticsearchService.IndexCampaignAsync(campaign2);
                    logger.LogInformation("Indexed demo campaigns into Elasticsearch.");
                }
                catch (Exception esEx)
                {
                    logger.LogWarning(esEx, "Elasticsearch indexing skipped during initial seeding (cluster may be offline).");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "DbInitializer completed with warnings. Application will continue: {Message}", ex.Message);
        }
    }
}
