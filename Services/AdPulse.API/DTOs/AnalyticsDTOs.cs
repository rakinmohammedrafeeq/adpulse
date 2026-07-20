namespace AdPulse.API.DTOs;

public record CampaignAnalyticsDto(
    Guid CampaignId,
    string CampaignName,
    int Impressions,
    int Clicks,
    int Conversions,
    decimal CTR, // Click-through rate
    decimal ConversionRate,
    decimal SpentAmount,
    decimal CPC, // Cost per click
    decimal CPA, // Cost per acquisition
    DateTime StartDate,
    DateTime EndDate
);

public record DashboardStatsDto(
    int TotalCampaigns,
    int ActiveCampaigns,
    decimal TotalBudget,
    decimal TotalSpent,
    int TotalImpressions,
    int TotalClicks,
    int TotalConversions,
    decimal AverageCTR,
    decimal AverageCPC,
    decimal TotalRevenue,
    decimal OverallROAS,
    List<CampaignPerformanceDto> TopCampaigns
);

public record CampaignPerformanceDto(
    Guid CampaignId,
    string CampaignName,
    int Impressions,
    int Clicks,
    int Conversions,
    decimal SpentAmount
);

public record TimeSeriesDataPoint(
    DateTime Date,
    int Impressions,
    int Clicks,
    int Conversions,
    decimal Spent
);
