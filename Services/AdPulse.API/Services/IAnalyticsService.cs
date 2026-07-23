using AdPulse.API.DTOs;

namespace AdPulse.API.Services;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(Guid tenantId);
    Task<CampaignAnalyticsDto> GetCampaignAnalyticsAsync(Guid campaignId, Guid tenantId, DateTime startDate, DateTime endDate);
    Task<List<TimeSeriesDataPoint>> GetCampaignTimeSeriesAsync(Guid campaignId, Guid tenantId, DateTime startDate, DateTime endDate);
}
