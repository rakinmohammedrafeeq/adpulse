using Microsoft.EntityFrameworkCore;
using AdPulse.API.Data;
using AdPulse.API.DTOs;
using AdPulse.API.Models;
using AdPulse.API.Services.Elasticsearch;

namespace AdPulse.API.Services;

public class CampaignService : ICampaignService
{
    private readonly AdPulseDbContext _context;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<CampaignService> _logger;

    public CampaignService(
        AdPulseDbContext context,
        IElasticsearchService elasticsearchService,
        ILogger<CampaignService> logger)
    {
        _context = context;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<List<CampaignListDto>> GetCampaignsAsync(Guid tenantId)
    {
        var campaigns = await _context.Campaigns
            .Where(c => c.TenantId == tenantId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CampaignListDto(
                c.Id,
                c.Name,
                c.Status,
                c.Objective,
                c.DailyBudget,
                c.SpentAmount,
                c.StartDate,
                c.EndDate,
                c.CreatedAt
            ))
            .ToListAsync();

        return campaigns;
    }

    public async Task<CampaignDetailDto?> GetCampaignByIdAsync(Guid id, Guid tenantId)
    {
        var campaign = await _context.Campaigns
            .Include(c => c.AdGroups)
            .ThenInclude(ag => ag.Creatives)
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (campaign == null)
        {
            return null;
        }

        return new CampaignDetailDto(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.Status,
            campaign.Objective,
            campaign.DailyBudget,
            campaign.TotalBudget,
            campaign.SpentAmount,
            campaign.StartDate,
            campaign.EndDate,
            campaign.CreatedAt,
            campaign.UpdatedAt,
            campaign.AdGroups.Select(ag => new AdGroupListDto(
                ag.Id,
                ag.Name,
                ag.Status,
                ag.BiddingStrategy,
                ag.BidAmount,
                ag.CreatedAt
            )).ToList()
        );
    }

    public async Task<CampaignDetailDto> CreateCampaignAsync(CreateCampaignRequest request, Guid tenantId, Guid userId)
    {
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            Objective = request.Objective,
            Status = CampaignStatus.Draft,
            DailyBudget = request.DailyBudget,
            TotalBudget = request.TotalBudget,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();

        // Index to Elasticsearch
        await _elasticsearchService.IndexCampaignAsync(campaign);

        _logger.LogInformation("Campaign created: {CampaignId} by user {UserId}", campaign.Id, userId);

        return new CampaignDetailDto(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.Status,
            campaign.Objective,
            campaign.DailyBudget,
            campaign.TotalBudget,
            campaign.SpentAmount,
            campaign.StartDate,
            campaign.EndDate,
            campaign.CreatedAt,
            campaign.UpdatedAt,
            new List<AdGroupListDto>()
        );
    }

    public async Task<CampaignDetailDto?> UpdateCampaignAsync(Guid id, UpdateCampaignRequest request, Guid tenantId)
    {
        var campaign = await _context.Campaigns
            .Include(c => c.AdGroups)
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (campaign == null)
        {
            return null;
        }

        if (request.Name != null) campaign.Name = request.Name;
        if (request.Description != null) campaign.Description = request.Description;
        if (request.Status.HasValue) campaign.Status = request.Status.Value;
        if (request.DailyBudget.HasValue) campaign.DailyBudget = request.DailyBudget.Value;
        if (request.TotalBudget.HasValue) campaign.TotalBudget = request.TotalBudget;
        if (request.StartDate.HasValue) campaign.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) campaign.EndDate = request.EndDate;

        campaign.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Update in Elasticsearch
        await _elasticsearchService.UpdateCampaignAsync(campaign);

        _logger.LogInformation("Campaign updated: {CampaignId}", campaign.Id);

        return new CampaignDetailDto(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.Status,
            campaign.Objective,
            campaign.DailyBudget,
            campaign.TotalBudget,
            campaign.SpentAmount,
            campaign.StartDate,
            campaign.EndDate,
            campaign.CreatedAt,
            campaign.UpdatedAt,
            campaign.AdGroups.Select(ag => new AdGroupListDto(
                ag.Id,
                ag.Name,
                ag.Status,
                ag.BiddingStrategy,
                ag.BidAmount,
                ag.CreatedAt
            )).ToList()
        );
    }

    public async Task<bool> DeleteCampaignAsync(Guid id, Guid tenantId)
    {
        var campaign = await _context.Campaigns
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

        if (campaign == null)
        {
            return false;
        }

        _context.Campaigns.Remove(campaign);
        await _context.SaveChangesAsync();

        // Delete from Elasticsearch
        await _elasticsearchService.DeleteCampaignAsync(id);

        _logger.LogInformation("Campaign deleted: {CampaignId}", id);

        return true;
    }
}
