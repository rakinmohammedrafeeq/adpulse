using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Entities;
using AdPulse.Core.Domain.Enums;
using AdPulse.Core.Domain.Events;
using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Shared.Constants;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Aggregates;

/// <summary>
/// ý����Դʵ�壨�ۺϸ���
/// </summary>
public class MediaResource : AggregateRoot
{
    /// <summary>
    /// ý����Դ����
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// ý������
    /// </summary>
    public MediaType Type { get; private set; }

    /// <summary>
    /// ý��URL
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// ý��״̬
    /// </summary>
    public MediaStatus Status { get; private set; }

    /// <summary>
    /// ������ID
    /// </summary>
    public Guid PublisherId { get; private set; }

    /// <summary>
    /// ���λ����
    /// </summary>
    public AdSlotConfiguration SlotConfig { get; private set; } = null!;

    /// <summary>
    /// �����ſ�
    /// </summary>
    public TrafficProfile TrafficProfile { get; private set; } = null!;

    /// <summary>
    /// ���λ����
    /// </summary>
    private readonly List<AdPlacement> _placements = new();
    public IReadOnlyList<AdPlacement> Placements => _placements.AsReadOnly();

    /// <summary>
    /// Ͷ�ż�¼����
    /// </summary>
    private readonly List<DeliveryRecord> _deliveryRecords = new();
    public IReadOnlyList<DeliveryRecord> DeliveryRecords => _deliveryRecords.AsReadOnly();

    /// <summary>
    /// ˽�й��캯��������ORM
    /// </summary>
    private MediaResource() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public MediaResource(
        string name,
        MediaType type,
        string url,
        Guid publisherId,
        AdSlotConfiguration slotConfig,
        TrafficProfile trafficProfile)
    {
        ValidateInputs(name, url, publisherId);

        Name = name;
        Type = type;
        Url = url;
        PublisherId = publisherId;
        SlotConfig = slotConfig;
        TrafficProfile = trafficProfile;
        Status = MediaStatus.Pending;

        // ����ý����Դ�����¼�
        AddDomainEvent(new MediaResourceCreatedEvent(Id, name, type, publisherId));
    }

    /// <summary>
    /// ���ù��λ
    /// </summary>
    public void Configure(AdSlotConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        SlotConfig = config;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceConfiguredEvent(Id));
    }

    /// <summary>
    /// ���������ſ�
    /// </summary>
    public void UpdateTrafficProfile(TrafficProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        TrafficProfile = profile;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceTrafficUpdatedEvent(Id));
    }

    /// <summary>
    /// ����ý����Դ
    /// </summary>
    public void Enable()
    {
        if (Status == MediaStatus.Active)
            throw new InvalidOperationException("ý����Դ�Ѿ���������״̬");

        Status = MediaStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceEnabledEvent(Id));
    }

    /// <summary>
    /// ����ý����Դ
    /// </summary>
    public void Disable()
    {
        if (Status == MediaStatus.Disabled)
            throw new InvalidOperationException("ý����Դ�Ѿ����ڽ���״̬");

        Status = MediaStatus.Disabled;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceDisabledEvent(Id));
    }

    /// <summary>
    /// ���ͨ��
    /// </summary>
    public void Approve()
    {
        if (Status != MediaStatus.Pending)
            throw new InvalidOperationException("ֻ�д����״̬��ý����Դ�������ͨ��");

        Status = MediaStatus.Active;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceApprovedEvent(Id));
    }

    /// <summary>
    /// ��˾ܾ�
    /// </summary>
    public void Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("�ܾ������ṩԭ��", nameof(reason));

        if (Status != MediaStatus.Pending)
            throw new InvalidOperationException("ֻ�д����״̬��ý����Դ���Ծܾ�");

        Status = MediaStatus.Rejected;
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceRejectedEvent(Id, reason));
    }

    /// <summary>
    /// ��ӹ��λ
    /// </summary>
    public void AddPlacement(AdPlacement placement)
    {
        ArgumentNullException.ThrowIfNull(placement);

        if (placement.MediaResourceId != Id)
            throw new ArgumentException("���λ����ý����ԴID��ƥ��");

        _placements.Add(placement);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourcePlacementAddedEvent(Id, placement.Id));
    }

    /// <summary>
    /// �Ƴ����λ
    /// </summary>
    public void RemovePlacement(Guid placementId)
    {
        if (placementId == Guid.Empty)
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        var placement = _placements.FirstOrDefault(p => p.Id == placementId);
        if (placement == null)
            throw new ArgumentException("δ�ҵ�ָ���Ĺ��λ", nameof(placementId));

        _placements.Remove(placement);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourcePlacementRemovedEvent(Id, placementId));
    }

    /// <summary>
    /// ��ȡ���ÿ��
    /// </summary>
    public int GetAvailableInventory()
    {
        if (Status != MediaStatus.Active)
            return 0;

        return TrafficProfile.DailyInventory - GetTodayDeliveryCount();
    }

    /// <summary>
    /// ��ȡ����Ͷ�Ŵ���
    /// </summary>
    public int GetTodayDeliveryCount()
    {
        var today = DateTime.UtcNow.Date;
        return _deliveryRecords
            .Where(r => r.DeliveredAt.Date == today)
            .Sum(r => (int)r.Metrics.Impressions);
    }

    /// <summary>
    /// ��¼Ͷ��
    /// </summary>
    public void RecordDelivery(DeliveryRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (record.MediaResourceId != Id)
            throw new ArgumentException("Ͷ�ż�¼��ý����ԴID��ƥ��");

        if (Status != MediaStatus.Active)
            throw new InvalidOperationException("ֻ�м���״̬��ý����Դ���Լ�¼Ͷ��");

        _deliveryRecords.Add(record);
        UpdateLastModifiedTime();
        AddDomainEvent(new MediaResourceDeliveryRecordedEvent(Id, record.Id));
    }

    /// <summary>
    /// ��ȡ����ͳ��
    /// </summary>
    public MediaPerformanceStats GetPerformanceStats()
    {
        var totalImpressions = _deliveryRecords.Sum(r => r.Metrics.Impressions);
        var totalClicks = _deliveryRecords.Sum(r => r.Metrics.Clicks);
        var totalRevenue = _deliveryRecords.Sum(r => r.Cost * SlotConfig.RevenueShareRate);
        var fillRate = TrafficProfile.DailyInventory > 0 ? (decimal)GetTodayDeliveryCount() / TrafficProfile.DailyInventory : 0m;

        return MediaPerformanceStats.Create(
            totalImpressions,
            totalClicks,
            totalRevenue,
            fillRate
        );
    }

    /// <summary>
    /// �Ƿ����Ͷ��
    /// </summary>
    public bool CanDeliver => Status == MediaStatus.Active &&
                             GetAvailableInventory() > 0 &&
                             !IsDeleted;

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string name, string url, Guid publisherId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("ý����Դ���Ʋ���Ϊ��", nameof(name));

        if (name.Length > ValidationConstants.StringLength.MediaNameMaxLength)
            throw new ArgumentException($"ý����Դ���Ƴ��Ȳ��ܳ���{ValidationConstants.StringLength.MediaNameMaxLength}���ַ�", nameof(name));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("ý��URL����Ϊ��", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("ý��URL��ʽ����ȷ", nameof(url));

        if (publisherId == Guid.Empty)
            throw new ArgumentException("������ID����Ϊ��", nameof(publisherId));
    }
}






