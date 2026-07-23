using AdPulse.API.DTOs;

namespace AdPulse.API.Services;

public interface ICampaignService
{
    Task<List<CampaignListDto>> GetCampaignsAsync(Guid tenantId);
    Task<CampaignDetailDto?> GetCampaignByIdAsync(Guid id, Guid tenantId);
    Task<CampaignDetailDto> CreateCampaignAsync(CreateCampaignRequest request, Guid tenantId, Guid userId);
    Task<CampaignDetailDto?> UpdateCampaignAsync(Guid id, UpdateCampaignRequest request, Guid tenantId);
    Task<bool> DeleteCampaignAsync(Guid id, Guid tenantId);
}
