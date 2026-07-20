using System.ComponentModel.DataAnnotations;
using AdPulse.API.Models;

namespace AdPulse.API.DTOs;

public record CampaignListDto(
    Guid Id,
    string Name,
    CampaignStatus Status,
    CampaignObjective Objective,
    decimal DailyBudget,
    decimal SpentAmount,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime CreatedAt
);

public record CampaignDetailDto(
    Guid Id,
    string Name,
    string? Description,
    CampaignStatus Status,
    CampaignObjective Objective,
    decimal DailyBudget,
    decimal? TotalBudget,
    decimal SpentAmount,
    DateTime StartDate,
    DateTime? EndDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<AdGroupListDto> AdGroups
);

public record CreateCampaignRequest(
    [Required] string Name,
    string? Description,
    [Required] CampaignObjective Objective,
    [Required][Range(0.01, double.MaxValue)] decimal DailyBudget,
    decimal? TotalBudget,
    [Required] DateTime StartDate,
    DateTime? EndDate
);

public record UpdateCampaignRequest(
    string? Name,
    string? Description,
    CampaignStatus? Status,
    decimal? DailyBudget,
    decimal? TotalBudget,
    DateTime? StartDate,
    DateTime? EndDate
);

public record AdGroupListDto(
    Guid Id,
    string Name,
    AdGroupStatus Status,
    BiddingStrategy BiddingStrategy,
    decimal BidAmount,
    DateTime CreatedAt
);

public record AdGroupDetailDto(
    Guid Id,
    string Name,
    AdGroupStatus Status,
    BiddingStrategy BiddingStrategy,
    decimal BidAmount,
    string? TargetingRules,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<CreativeListDto> Creatives
);

public record CreateAdGroupRequest(
    [Required] string Name,
    [Required] BiddingStrategy BiddingStrategy,
    [Required][Range(0.01, double.MaxValue)] decimal BidAmount,
    string? TargetingRules
);

public record UpdateAdGroupRequest(
    string? Name,
    AdGroupStatus? Status,
    BiddingStrategy? BiddingStrategy,
    decimal? BidAmount,
    string? TargetingRules
);

public record CreativeListDto(
    Guid Id,
    string Name,
    CreativeType Type,
    CreativeStatus Status,
    string Headline,
    string? ImageUrl,
    DateTime CreatedAt
);

public record CreativeDetailDto(
    Guid Id,
    string Name,
    CreativeType Type,
    CreativeStatus Status,
    string Headline,
    string? Description,
    string? ImageUrl,
    string? VideoUrl,
    string DestinationUrl,
    string? CallToAction,
    int? Width,
    int? Height,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateCreativeRequest(
    [Required] string Name,
    [Required] CreativeType Type,
    [Required] string Headline,
    string? Description,
    string? ImageUrl,
    string? VideoUrl,
    [Required][Url] string DestinationUrl,
    string? CallToAction,
    int? Width,
    int? Height
);

public record UpdateCreativeRequest(
    string? Name,
    CreativeStatus? Status,
    string? Headline,
    string? Description,
    string? ImageUrl,
    string? VideoUrl,
    string? DestinationUrl,
    string? CallToAction
);
