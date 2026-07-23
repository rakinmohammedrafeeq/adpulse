using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using AdPulse.API.Models;

namespace AdPulse.API.Services.Elasticsearch;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string CampaignsIndex = "adpulse-campaigns";
    private const string EventsIndex = "adpulse-events";

    public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService> logger)
    {
        _logger = logger;

        var elasticsearchUri = Environment.GetEnvironmentVariable("ELASTICSEARCH_URI")
            ?? configuration["Elasticsearch:Uri"]
            ?? "http://localhost:9200";

        var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri))
            .DefaultIndex(CampaignsIndex)
            .RequestTimeout(TimeSpan.FromSeconds(10));

        _client = new ElasticsearchClient(settings);
        _logger.LogInformation("Elasticsearch client initialized with URI: {Uri}", elasticsearchUri);
    }

    public async Task InitializeAsync()
    {
        try
        {
            var ping = await _client.PingAsync();
            if (!ping.IsValidResponse)
            {
                _logger.LogWarning("Elasticsearch cluster ping failed. Search features will be limited until cluster is available.");
                return;
            }

            // Create campaigns index if it doesn't exist
            var campaignsExistsResponse = await _client.Indices.ExistsAsync(CampaignsIndex);
            if (!campaignsExistsResponse.Exists)
            {
                await CreateCampaignsIndexAsync();
            }

            // Create events index if it doesn't exist
            var eventsExistsResponse = await _client.Indices.ExistsAsync(EventsIndex);
            if (!eventsExistsResponse.Exists)
            {
                await CreateEventsIndexAsync();
            }

            _logger.LogInformation("Elasticsearch indices initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not initialize Elasticsearch at startup. Continuing API startup.");
        }
    }

    private async Task CreateCampaignsIndexAsync()
    {
        var createIndexResponse = await _client.Indices.CreateAsync(CampaignsIndex, c => c
            .Mappings(m => m
                .Properties<Campaign>(p => p
                    .Keyword(k => k.Id)
                    .Keyword(k => k.TenantId)
                    .Text(t => t.Name)
                    .Text(t => t.Description)
                    .Keyword(k => k.Status)
                    .Keyword(k => k.Objective)
                    .DoubleNumber(n => n.DailyBudget)
                    .DoubleNumber(n => n.TotalBudget)
                    .DoubleNumber(n => n.SpentAmount)
                    .Date(d => d.StartDate)
                    .Date(d => d.EndDate)
                    .Date(d => d.CreatedAt)
                    .Date(d => d.UpdatedAt)
                )
            )
        );

        if (createIndexResponse.IsValidResponse)
        {
            _logger.LogInformation("Created Elasticsearch index: {Index}", CampaignsIndex);
        }
        else
        {
            _logger.LogWarning("Index creation for {Index} reported: {Error}",
                CampaignsIndex, createIndexResponse.DebugInformation);
        }
    }

    private async Task CreateEventsIndexAsync()
    {
        var createIndexResponse = await _client.Indices.CreateAsync(EventsIndex, c => c
            .Mappings(m => m
                .Properties<AdEvent>(p => p
                    .Keyword(k => k.Id)
                    .Keyword(k => k.TenantId)
                    .Keyword(k => k.CampaignId)
                    .Keyword(k => k.AdGroupId)
                    .Keyword(k => k.CreativeId)
                    .Keyword(k => k.EventType)
                    .Date(d => d.EventTime)
                    .Keyword(k => k.UserId)
                    .Keyword(k => k.SessionId)
                    .Ip(i => i.IpAddress)
                    .Keyword(k => k.DeviceType)
                    .Keyword(k => k.Country)
                    .Keyword(k => k.City)
                    .DoubleNumber(n => n.ConversionValue)
                )
            )
        );

        if (createIndexResponse.IsValidResponse)
        {
            _logger.LogInformation("Created Elasticsearch index: {Index}", EventsIndex);
        }
        else
        {
            _logger.LogWarning("Index creation for {Index} reported: {Error}",
                EventsIndex, createIndexResponse.DebugInformation);
        }
    }

    public async Task IndexCampaignAsync(Campaign campaign)
    {
        try
        {
            var response = await _client.IndexAsync(campaign, idx => idx.Index(CampaignsIndex).Id(campaign.Id.ToString()));

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Indexed campaign {CampaignId} to Elasticsearch", campaign.Id);
            }
            else
            {
                _logger.LogWarning("Failed to index campaign {CampaignId}: {Error}",
                    campaign.Id, response.DebugInformation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error indexing campaign {CampaignId}", campaign.Id);
        }
    }

    public async Task UpdateCampaignAsync(Campaign campaign)
    {
        await IndexCampaignAsync(campaign);
    }

    public async Task DeleteCampaignAsync(Guid campaignId)
    {
        try
        {
            var response = await _client.DeleteAsync(new DeleteRequest(CampaignsIndex, (Id)campaignId.ToString()));

            if (response.IsValidResponse)
            {
                _logger.LogInformation("Deleted campaign {CampaignId} from Elasticsearch", campaignId);
            }
            else
            {
                _logger.LogWarning("Campaign {CampaignId} not found in Elasticsearch or delete failed", campaignId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error deleting campaign {CampaignId}", campaignId);
        }
    }

    public async Task<List<Campaign>> SearchCampaignsAsync(string query, Guid tenantId, int from = 0, int size = 10)
    {
        try
        {
            var searchResponse = await _client.SearchAsync<Campaign>(s => s
                .Index(CampaignsIndex)
                .From(from)
                .Size(size)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.Term(t => t.Field(f => f.TenantId).Value(tenantId.ToString()))
                        )
                        .Should(
                            sh => sh.Match(m => m.Field(f => f.Name).Query(query).Boost(2)),
                            sh => sh.Match(m => m.Field(f => f.Description!).Query(query)),
                            sh => sh.Wildcard(w => w.Field(f => f.Name).Value($"*{query.ToLower()}*"))
                        )
                        .MinimumShouldMatch(1)
                    )
                )
                .Sort(so => so
                    .Field(f => f.CreatedAt, new FieldSort { Order = SortOrder.Desc })
                )
            );

            if (searchResponse.IsValidResponse)
            {
                _logger.LogInformation("Search found {Count} campaigns for query: {Query}",
                    searchResponse.Documents.Count, query);
                return searchResponse.Documents.ToList();
            }

            _logger.LogWarning("Search failed for query: {Query}, Error: {Error}",
                query, searchResponse.DebugInformation);
            return new List<Campaign>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error searching campaigns with query: {Query}", query);
            return new List<Campaign>();
        }
    }

    public async Task IndexEventAsync(AdEvent adEvent)
    {
        try
        {
            var response = await _client.IndexAsync(adEvent, idx => idx.Index(EventsIndex).Id(adEvent.Id.ToString()));

            if (response.IsValidResponse)
            {
                _logger.LogDebug("Indexed event {EventId} to Elasticsearch", adEvent.Id);
            }
            else
            {
                _logger.LogWarning("Failed to index event {EventId}: {Error}",
                    adEvent.Id, response.DebugInformation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error indexing event {EventId}", adEvent.Id);
        }
    }

    public async Task<Dictionary<string, long>> GetEventStatisticsAsync(
        Guid tenantId,
        Guid campaignId,
        DateTime startDate,
        DateTime endDate)
    {
        try
        {
            var searchResponse = await _client.SearchAsync<AdEvent>(s => s
                .Index(EventsIndex)
                .Size(0)
                .Query(q => q
                    .Bool(b => b
                        .Filter(
                            f => f.Term(t => t.Field(fld => fld.TenantId).Value(tenantId.ToString())),
                            f => f.Term(t => t.Field(fld => fld.CampaignId).Value(campaignId.ToString())),
                            f => f.Range(r => r
                                .DateRange(dr => dr
                                    .Field(fld => fld.EventTime)
                                    .Gte(startDate)
                                    .Lte(endDate)
                                )
                            )
                        )
                    )
                )
                .Aggregations(a => a
                    .Add("event_types", agg => agg
                        .Terms(t => t.Field(f => f.EventType))
                    )
                )
            );

            var stats = new Dictionary<string, long>();

            if (searchResponse.IsValidResponse && searchResponse.Aggregations != null)
            {
                var termsAgg = searchResponse.Aggregations.GetStringTerms("event_types");
                if (termsAgg != null)
                {
                    foreach (var bucket in termsAgg.Buckets)
                    {
                        if (bucket.Key != null)
                        {
                            stats[bucket.Key.ToString()!] = bucket.DocCount;
                        }
                    }
                }

                _logger.LogInformation("Retrieved event statistics for campaign {CampaignId}", campaignId);
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting event statistics for campaign {CampaignId}", campaignId);
            return new Dictionary<string, long>();
        }
    }
}
