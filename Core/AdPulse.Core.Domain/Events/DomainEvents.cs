using AdPulse.Core.Domain.Common;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Events;

/// <summary>
/// �����¼�����
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// �¼�ID
    /// </summary>
    public string EventId { get; } = Guid.NewGuid().ToString();

    /// <summary>
    /// �¼�����ʱ��
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// �¼�����
    /// </summary>
    public abstract string EventType { get; }
}

#region Advertisement Events

/// <summary>
/// ��洴���¼�
/// </summary>
public class AdvertisementCreatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementCreated";

    public string AdvertisementId { get; }
    public string AdvertiserId { get; }
    public string CampaignId { get; }
    public string AdvertisementName { get; }

    public AdvertisementCreatedEvent(string advertisementId, string advertiserId, string campaignId, string advertisementName)
    {
        AdvertisementId = advertisementId;
        AdvertiserId = advertiserId;
        CampaignId = campaignId;
        AdvertisementName = advertisementName;
    }
}

/// <summary>
/// �������¼�
/// </summary>
public class AdvertisementUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementUpdated";

    public string AdvertisementId { get; }
    public string UpdateType { get; }

    public AdvertisementUpdatedEvent(string advertisementId, string updateType)
    {
        AdvertisementId = advertisementId;
        UpdateType = updateType;
    }
}

/// <summary>
/// ����ύ����¼�
/// </summary>
public class AdvertisementSubmittedForAuditEvent : DomainEventBase
{
    public override string EventType => "AdvertisementSubmittedForAudit";

    public string AdvertisementId { get; }
    public string AdvertiserId { get; }

    public AdvertisementSubmittedForAuditEvent(string advertisementId, string advertiserId)
    {
        AdvertisementId = advertisementId;
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// �����˿�ʼ�¼�
/// </summary>
public class AdvertisementAuditStartedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditStarted";

    public string AdvertisementId { get; }
    public string AuditorId { get; }

    public AdvertisementAuditStartedEvent(string advertisementId, string auditorId)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
    }
}

/// <summary>
/// ������ͨ���¼�
/// </summary>
public class AdvertisementAuditApprovedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditApproved";

    public string AdvertisementId { get; }
    public string AuditorId { get; }

    public AdvertisementAuditApprovedEvent(string advertisementId, string auditorId)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
    }
}

/// <summary>
/// �����˾ܾ��¼�
/// </summary>
public class AdvertisementAuditRejectedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAuditRejected";

    public string AdvertisementId { get; }
    public string AuditorId { get; }
    public string Feedback { get; }

    public AdvertisementAuditRejectedEvent(string advertisementId, string auditorId, string feedback)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
        Feedback = feedback;
    }
}

/// <summary>
/// �����Ҫ�޸��¼�
/// </summary>
public class AdvertisementRequiresChangesEvent : DomainEventBase
{
    public override string EventType => "AdvertisementRequiresChanges";

    public string AdvertisementId { get; }
    public string AuditorId { get; }
    public string CorrectionSuggestion { get; }

    public AdvertisementRequiresChangesEvent(string advertisementId, string auditorId, string correctionSuggestion)
    {
        AdvertisementId = advertisementId;
        AuditorId = auditorId;
        CorrectionSuggestion = correctionSuggestion;
    }
}

/// <summary>
/// ��漤���¼�
/// </summary>
public class AdvertisementActivatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementActivated";

    public string AdvertisementId { get; }

    public AdvertisementActivatedEvent(string advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// �����ͣ�¼�
/// </summary>
public class AdvertisementPausedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementPaused";

    public string AdvertisementId { get; }

    public AdvertisementPausedEvent(string advertisementId)
    {
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// ���չʾ��¼�¼�
/// </summary>
public class AdvertisementImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementImpressionRecorded";

    public string AdvertisementId { get; }
    public decimal Cost { get; }

    public AdvertisementImpressionRecordedEvent(string advertisementId, decimal cost)
    {
        AdvertisementId = advertisementId;
        Cost = cost;
    }
}

/// <summary>
/// �������¼�¼�
/// </summary>
public class AdvertisementClickRecordedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementClickRecorded";

    public string AdvertisementId { get; }
    public decimal Cost { get; }

    public AdvertisementClickRecordedEvent(string advertisementId, decimal cost)
    {
        AdvertisementId = advertisementId;
        Cost = cost;
    }
}

/// <summary>
/// ��������÷ָ����¼�
/// </summary>
public class AdvertisementQualityScoreUpdatedEvent : DomainEventBase
{
    public override string EventType => "AdvertisementQualityScoreUpdated";

    public string AdvertisementId { get; }
    public decimal OldScore { get; }
    public decimal NewScore { get; }

    public AdvertisementQualityScoreUpdatedEvent(string advertisementId, decimal oldScore, decimal newScore)
    {
        AdvertisementId = advertisementId;
        OldScore = oldScore;
        NewScore = newScore;
    }
}

#endregion

#region Campaign Events

/// <summary>
/// ������¼�
/// </summary>
public class CampaignCreatedEvent : DomainEventBase
{
    public override string EventType => "CampaignCreated";

    public string CampaignId { get; }
    public string AdvertiserId { get; }
    public string CampaignName { get; }

    public CampaignCreatedEvent(string campaignId, string advertiserId, string campaignName)
    {
        CampaignId = campaignId;
        AdvertiserId = advertiserId;
        CampaignName = campaignName;
    }
}

/// <summary>
/// ������¼�
/// </summary>
public class CampaignUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignUpdated";

    public string CampaignId { get; }
    public string UpdateType { get; }

    public CampaignUpdatedEvent(string campaignId, string updateType)
    {
        CampaignId = campaignId;
        UpdateType = updateType;
    }
}

/// <summary>
/// �Ԥ������¼�
/// </summary>
public class CampaignBudgetUpdatedEvent : DomainEventBase
{
    public override string EventType => "CampaignBudgetUpdated";

    public string CampaignId { get; }
    public decimal TotalBudget { get; }
    public decimal DailyBudget { get; }

    public CampaignBudgetUpdatedEvent(string campaignId, decimal totalBudget, decimal dailyBudget)
    {
        CampaignId = campaignId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
    }
    public CampaignBudgetUpdatedEvent(string campaignId, decimal totalBudget)
    {
        CampaignId = campaignId;
        TotalBudget = totalBudget;
    }
}

/// <summary>
/// ��ύ����¼�
/// </summary>
public class CampaignSubmittedForReviewEvent : DomainEventBase
{
    public override string EventType => "CampaignSubmittedForReview";

    public string CampaignId { get; }
    public string AdvertiserId { get; }

    public CampaignSubmittedForReviewEvent(string campaignId, string advertiserId)
    {
        CampaignId = campaignId;
        AdvertiserId = advertiserId;
    }
}

/// <summary>
/// ������¼�
/// </summary>
public class CampaignActivatedEvent : DomainEventBase
{
    public override string EventType => "CampaignActivated";

    public string CampaignId { get; }

    public CampaignActivatedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// ���ͣ�¼�
/// </summary>
public class CampaignPausedEvent : DomainEventBase
{
    public override string EventType => "CampaignPaused";

    public string CampaignId { get; }

    public CampaignPausedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// ������¼�
/// </summary>
public class CampaignEndedEvent : DomainEventBase
{
    public override string EventType => "CampaignEnded";

    public string CampaignId { get; }

    public CampaignEndedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// �ɾ���¼�
/// </summary>
public class CampaignDeletedEvent : DomainEventBase
{
    public override string EventType => "CampaignDeleted";

    public string CampaignId { get; }

    public CampaignDeletedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// �����ӵ���¼�
/// </summary>
public class AdvertisementAddedToCampaignEvent : DomainEventBase
{
    public override string EventType => "AdvertisementAddedToCampaign";

    public string CampaignId { get; }
    public string AdvertisementId { get; }

    public AdvertisementAddedToCampaignEvent(string campaignId, string advertisementId)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// ���ӻ�Ƴ��¼�
/// </summary>
public class AdvertisementRemovedFromCampaignEvent : DomainEventBase
{
    public override string EventType => "AdvertisementRemovedFromCampaign";

    public string CampaignId { get; }
    public string AdvertisementId { get; }

    public AdvertisementRemovedFromCampaignEvent(string campaignId, string advertisementId)
    {
        CampaignId = campaignId;
        AdvertisementId = advertisementId;
    }
}

/// <summary>
/// �չʾ��¼�¼�
/// </summary>
public class CampaignImpressionRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignImpressionRecorded";

    public string CampaignId { get; }
    public decimal Cost { get; }

    public CampaignImpressionRecordedEvent(string campaignId, decimal cost)
    {
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// ������¼�¼�
/// </summary>
public class CampaignClickRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignClickRecorded";

    public string CampaignId { get; }
    public decimal Cost { get; }

    public CampaignClickRecordedEvent(string campaignId, decimal cost)
    {
        CampaignId = campaignId;
        Cost = cost;
    }
}

/// <summary>
/// �ת����¼�¼�
/// </summary>
public class CampaignConversionRecordedEvent : DomainEventBase
{
    public override string EventType => "CampaignConversionRecorded";

    public string CampaignId { get; }

    public CampaignConversionRecordedEvent(string campaignId)
    {
        CampaignId = campaignId;
    }
}

/// <summary>
/// �Ԥ��ľ��¼�
/// </summary>
public class CampaignBudgetExhaustedEvent : DomainEventBase
{
    public override string EventType => "CampaignBudgetExhausted";

    public string CampaignId { get; }
    public decimal SpentBudget { get; }

    public CampaignBudgetExhaustedEvent(string campaignId, decimal spentBudget)
    {
        CampaignId = campaignId;
        SpentBudget = spentBudget;
    }
}

#endregion

#region UserProfile Events

/// <summary>
/// �û����񴴽��¼�
/// </summary>
public class UserProfileCreatedEvent : DomainEventBase
{
    public override string EventType => "UserProfileCreated";

    public string UserProfileId { get; }
    public string UserId { get; }

    public UserProfileCreatedEvent(string userProfileId, string userId)
    {
        UserProfileId = userProfileId;
        UserId = userId;
    }
}

/// <summary>
/// �û���������¼�
/// </summary>
public class UserProfileUpdatedEvent : DomainEventBase
{
    public override string EventType => "UserProfileUpdated";

    public string UserProfileId { get; }
    public string UserId { get; }
    public string UpdateType { get; }

    public UserProfileUpdatedEvent(string userProfileId, string userId, string updateType)
    {
        UserProfileId = userProfileId;
        UserId = userId;
        UpdateType = updateType;
    }
}

/// <summary>
/// �û��������ָ����¼�
/// </summary>
public class UserProfileScoreUpdatedEvent : DomainEventBase
{
    public override string EventType => "UserProfileScoreUpdated";

    public string UserProfileId { get; }
    public string UserId { get; }
    public string ScoreType { get; }
    public int NewScore { get; }

    public UserProfileScoreUpdatedEvent(string userProfileId, string userId, string scoreType, int newScore)
    {
        UserProfileId = userProfileId;
        UserId = userId;
        ScoreType = scoreType;
        NewScore = newScore;
    }
}

/// <summary>
/// �û����¼�¼�
/// </summary>
public class UserActivityRecordedEvent : DomainEventBase
{
    public override string EventType => "UserActivityRecorded";

    public string UserProfileId { get; }
    public string UserId { get; }
    public DateTime ActivityTimestamp { get; }

    public UserActivityRecordedEvent(string userProfileId, string userId, DateTime activityTimestamp)
    {
        UserProfileId = userProfileId;
        UserId = userId;
        ActivityTimestamp = activityTimestamp;
    }
}

/// <summary>
/// �û�����״̬����¼�
/// </summary>
public class UserProfileStatusChangedEvent : DomainEventBase
{
    public override string EventType => "UserProfileStatusChanged";

    public string UserProfileId { get; }
    public string UserId { get; }
    public UserStatus NewStatus { get; }

    public UserProfileStatusChangedEvent(string userProfileId, string userId, UserStatus newStatus)
    {
        UserProfileId = userProfileId;
        UserId = userId;
        NewStatus = newStatus;
    }
}

#endregion