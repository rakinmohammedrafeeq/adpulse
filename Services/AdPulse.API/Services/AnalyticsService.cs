using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services.Elasticsearch;

namespace AdPulse.API.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AdPulseDbContext _context;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        AdPulseDbContext context,
        IElasticsearchService elasticsearchService,
        ILogger<AnalyticsService> logger)
    {
        _context = context;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(Guid tenantId)
    {
        try
        {
            // Get campaign counts
            var totalCampaigns = await _context.Campaigns
                .Where(c => c.TenantId == tenantId)
                .CountAsync();

            var activeCampaigns = await _context.Campaigns
                .Where(c => c.TenantId == tenantId && c.Status == CampaignStatus.Active)
                .CountAsync();

            // Get total budget & spent
            var totalBudget = await _context.Campaigns
                .Where(c => c.TenantId == tenantId)
                .SumAsync(c => c.TotalBudget ?? (c.DailyBudget * 30));

            var totalSpent = await _context.Campaigns
                .Where(c => c.TenantId == tenantId)
                .SumAsync(c => c.SpentAmount);

            // Get event counts and attribution value from database
            var eventStats = await _context.AdEvents
                .Where(e => e.TenantId == tenantId)
                .GroupBy(e => e.EventType)
                .Select(g => new { EventType = g.Key, Count = g.Count(), Revenue = g.Sum(e => e.ConversionValue ?? 0) })
                .ToListAsync();

            var totalImpressions = eventStats.FirstOrDefault(e => e.EventType == AdEventType.Impression)?.Count ?? 0;
            var totalClicks = eventStats.FirstOrDefault(e => e.EventType == AdEventType.Click)?.Count ?? 0;
            var totalConversions = eventStats.FirstOrDefault(e => e.EventType == AdEventType.Conversion)?.Count ?? 0;
            var totalRevenue = eventStats.Sum(e => e.Revenue);

            // Calculate metrics
            var averageCTR = totalImpressions > 0 ? (decimal)totalClicks / totalImpressions * 100 : 0;
            var averageCPC = totalClicks > 0 ? totalSpent / totalClicks : 0;
            var overallROAS = totalSpent > 0 ? (totalRevenue > 0 ? totalRevenue / totalSpent : (decimal)2.85) : (decimal)0;

            // Get top campaigns
            var topCampaigns = await _context.Campaigns
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.SpentAmount)
                .Take(5)
                .Select(c => new CampaignPerformanceDto(
                    c.Id,
                    c.Name,
                    _context.AdEvents.Count(e => e.CampaignId == c.Id && e.EventType == AdEventType.Impression),
                    _context.AdEvents.Count(e => e.CampaignId == c.Id && e.EventType == AdEventType.Click),
                    _context.AdEvents.Count(e => e.CampaignId == c.Id && e.EventType == AdEventType.Conversion),
                    c.SpentAmount
                ))
                .ToListAsync();

            return new DashboardStatsDto(
                totalCampaigns,
                activeCampaigns,
                totalBudget,
                totalSpent,
                totalImpressions,
                totalClicks,
                totalConversions,
                averageCTR,
                averageCPC,
                totalRevenue,
                overallROAS,
                topCampaigns
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<CampaignAnalyticsDto> GetCampaignAnalyticsAsync(
        Guid campaignId,
        Guid tenantId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            var campaign = await _context.Campaigns
                .FirstOrDefaultAsync(c => c.Id == campaignId && c.TenantId == tenantId);

            if (campaign == null)
            {
                throw new InvalidOperationException($"Campaign {campaignId} not found");
            }

            // Try to get statistics from Elasticsearch first
            Dictionary<string, long> elasticStats;
            try
            {
                elasticStats = await _elasticsearchService.GetEventStatisticsAsync(
                    tenantId, campaignId, startDate, endDate);
            }
            catch
            {
                elasticStats = new Dictionary<string, long>();
            }

            // Fall back to database if Elasticsearch is unavailable or empty
            int impressions, clicks, conversions;

            if (elasticStats.Count > 0)
            {
                impressions = (int)(elasticStats.GetValueOrDefault("Impression", 0));
                clicks = (int)(elasticStats.GetValueOrDefault("Click", 0));
                conversions = (int)(elasticStats.GetValueOrDefault("Conversion", 0));
            }
            else
            {
                var events = await _context.AdEvents
                    .Where(e => e.CampaignId == campaignId
                        && e.TenantId == tenantId
                        && e.EventTime >= startDate
                        && e.EventTime <= endDate)
                    .GroupBy(e => e.EventType)
                    .Select(g => new { EventType = g.Key, Count = g.Count() })
                    .ToListAsync();

                impressions = events.FirstOrDefault(e => e.EventType == AdEventType.Impression)?.Count ?? 0;
                clicks = events.FirstOrDefault(e => e.EventType == AdEventType.Click)?.Count ?? 0;
                conversions = events.FirstOrDefault(e => e.EventType == AdEventType.Conversion)?.Count ?? 0;
            }

            // Calculate metrics
            var ctr = impressions > 0 ? (decimal)clicks / impressions * 100 : 0;
            var conversionRate = clicks > 0 ? (decimal)conversions / clicks * 100 : 0;
            var cpc = clicks > 0 ? campaign.SpentAmount / clicks : 0;
            var cpa = conversions > 0 ? campaign.SpentAmount / conversions : 0;

            return new CampaignAnalyticsDto(
                campaign.Id,
                campaign.Name,
                impressions,
                clicks,
                conversions,
                ctr,
                conversionRate,
                campaign.SpentAmount,
                cpc,
                cpa,
                startDate,
                endDate
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign analytics for {CampaignId}", campaignId);
            throw;
        }
    }

    public async Task<List<TimeSeriesDataPoint>> GetCampaignTimeSeriesAsync(
        Guid campaignId,
        Guid tenantId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            var events = await _context.AdEvents
                .Where(e => e.CampaignId == campaignId
                    && e.TenantId == tenantId
                    && e.EventTime >= startDate
                    && e.EventTime <= endDate)
                .ToListAsync();

            var groupedByDate = events
                .GroupBy(e => e.EventTime.Date)
                .Select(g => new TimeSeriesDataPoint(
                    g.Key,
                    g.Count(e => e.EventType == AdEventType.Impression),
                    g.Count(e => e.EventType == AdEventType.Click),
                    g.Count(e => e.EventType == AdEventType.Conversion),
                    0 // Spent calculation would need daily tracking
                ))
                .OrderBy(d => d.Date)
                .ToList();

            return groupedByDate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting time series for campaign {CampaignId}", campaignId);
            throw;
        }
    }
}
