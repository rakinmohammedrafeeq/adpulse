using AdPulse.API.Models;

namespace AdPulse.API.Services.Elasticsearch;

public interface IElasticsearchService
{
    Task InitializeAsync();
    Task IndexCampaignAsync(Campaign campaign);
    Task UpdateCampaignAsync(Campaign campaign);
    Task DeleteCampaignAsync(Guid campaignId);
    Task<List<Campaign>> SearchCampaignsAsync(string query, Guid tenantId, int from = 0, int size = 10);
    Task IndexEventAsync(AdEvent adEvent);
    Task<Dictionary<string, long>> GetEventStatisticsAsync(Guid tenantId, Guid campaignId, DateTime startDate, DateTime endDate);
}
