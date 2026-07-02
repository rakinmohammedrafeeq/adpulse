using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Events;
using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Aggregates;

/// <summary>
/// Ͷ�ż�¼�ۺϸ�
/// </summary>
public class DeliveryRecord : AggregateRoot
{
    /// <summary>
    /// ���ID
    /// </summary>
    public Guid AdId { get; private set; }

    /// <summary>
    /// �ID
    /// </summary>
    public Guid CampaignId { get; private set; }

    /// <summary>
    /// ý����ԴID
    /// </summary>
    public Guid MediaResourceId { get; private set; }

    /// <summary>
    /// ���λID
    /// </summary>
    public Guid PlacementId { get; private set; }

    /// <summary>
    /// չʾΨһID
    /// </summary>
    public string ImpressionId { get; private set; } = string.Empty;

    /// <summary>
    /// ����ID
    /// </summary>
    public string RequestId { get; private set; } = string.Empty;

    /// <summary>
    /// Ͷ��ʱ���
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Ͷ��ʱ�䣨����ҵ���ѯ��ͳ�ƣ�
    /// </summary>
    public DateTime DeliveredAt { get; private set; }

    /// <summary>
    /// ʵ�ʼ۸񣨷֣�
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Ͷ�ųɱ����֣� - ����ý����Դ�������
    /// </summary>
    public decimal Cost { get; private set; }

    /// <summary>
    /// Ͷ��״̬
    /// </summary>
    public DeliveryStatus Status { get; private set; }

    /// <summary>
    /// ����ָ��
    /// </summary>
    public PerformanceMetrics Metrics { get; private set; } = null!;

    /// <summary>
    /// Ͷ�������ļ���
    /// ʹ��ITargetingContext�ӿ��ṩ����չ����������Ϣ�洢
    /// </summary>
    public IReadOnlyDictionary<string, ITargetingContext> DeliveryContexts { get; private set; }

    /// <summary>
    /// ���ʱ��
    /// </summary>
    public DateTime? ClickTime { get; private set; }

    /// <summary>
    /// ת��ʱ��
    /// </summary>
    public DateTime? ConversionTime { get; private set; }

    /// <summary>
    /// ת�����루�֣�
    /// </summary>
    public decimal? Revenue { get; private set; }

    /// <summary>
    /// �����÷�
    /// </summary>
    public decimal QualityScore { get; private set; } = 1.0m;

    /// <summary>
    /// ���ۼ۸񣨷֣�
    /// </summary>
    public decimal BidPrice { get; private set; }

    /// <summary>
    /// �Ʒ�������ϸ
    /// </summary>
    public BillingDetails BillingDetails { get; private set; } = null!;

    /// <summary>
    /// ˽�й��캯��������ORM
    /// </summary>
    private DeliveryRecord()
    {
        DeliveryContexts = new Dictionary<string, ITargetingContext>();
        Metrics = PerformanceMetrics.Create();
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public DeliveryRecord(
        Guid adId,
        Guid campaignId,
        Guid mediaResourceId,
        Guid placementId,
        string impressionId,
        string requestId,
        decimal price,
        decimal cost,
        decimal bidPrice,
        IReadOnlyDictionary<string, ITargetingContext> deliveryContexts,
        BillingDetails billingDetails,
        decimal qualityScore = 1.0m)
    {
        ValidateInputs(adId, campaignId, mediaResourceId, placementId, impressionId, requestId, price, cost, bidPrice, deliveryContexts, billingDetails);

        AdId = adId;
        CampaignId = campaignId;
        MediaResourceId = mediaResourceId;
        PlacementId = placementId;
        ImpressionId = impressionId;
        RequestId = requestId;
        Price = price;
        Cost = cost;
        BidPrice = bidPrice;
        DeliveryContexts = deliveryContexts;
        BillingDetails = billingDetails;
        QualityScore = qualityScore;
        Timestamp = DateTime.UtcNow;
        DeliveredAt = DateTime.UtcNow;
        Status = DeliveryStatus.Success;
        Metrics = PerformanceMetrics.Create();

        // ����Ͷ�ż�¼�����¼�
        AddDomainEvent(new DeliveryRecordCreatedEvent(Id, Guid.Parse(RequestId), CampaignId, cost));
    }

    /// <summary>
    /// ��¼չʾ
    /// </summary>
    public void RecordImpression()
    {
        if (Status != DeliveryStatus.Success)
            throw new InvalidOperationException("ֻ�гɹ�Ͷ��״̬�ļ�¼���Լ�¼չʾ");

        Metrics = Metrics.RecordImpression();
        UpdateLastModifiedTime();
        AddDomainEvent(new ImpressionRecordedEvent(Id, Guid.Parse(RequestId), CampaignId));
    }

    /// <summary>
    /// ��¼���
    /// </summary>
    public void RecordClick(decimal? clickCost = null)
    {
        if (ClickTime.HasValue)
            throw new InvalidOperationException("��Ͷ�ż�¼�Ѽ�¼�����");

        if (Metrics.Impressions == 0)
            throw new InvalidOperationException("��������չʾ���ܼ�¼���");

        ClickTime = DateTime.UtcNow;
        Metrics = Metrics.RecordClick();

        if (clickCost.HasValue)
        {
            Price += clickCost.Value;
            Cost += clickCost.Value;
            BillingDetails = BillingDetails.AddCost(clickCost.Value, "�������");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryClickRecordedEvent(Id, AdId, clickCost ?? 0));
    }

    /// <summary>
    /// ��¼ת��
    /// </summary>
    public void RecordConversion(decimal? revenue = null, decimal? conversionCost = null)
    {
        if (ConversionTime.HasValue)
            throw new InvalidOperationException("��Ͷ�ż�¼�Ѽ�¼��ת��");

        if (Metrics.Clicks == 0)
            throw new InvalidOperationException("�������е�����ܼ�¼ת��");

        ConversionTime = DateTime.UtcNow;
        Revenue = revenue;
        Metrics = Metrics.RecordConversion();

        if (conversionCost.HasValue)
        {
            Price += conversionCost.Value;
            Cost += conversionCost.Value;
            BillingDetails = BillingDetails.AddCost(conversionCost.Value, "ת������");
        }

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryConversionRecordedEvent(Id, AdId, revenue ?? 0));
    }

    /// <summary>
    /// ����Ͷ��״̬
    /// </summary>
    public void UpdateStatus(DeliveryStatus status)
    {
        if (Status == status)
            return;

        var oldStatus = Status;
        Status = status;

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryStatusUpdatedEvent(Id, status));
    }

    /// <summary>
    /// ���Ϊʧ��
    /// </summary>
    public void MarkAsFailed(string reason)
    {
        Status = DeliveryStatus.Failed;
        BillingDetails = BillingDetails.AddNote($"ʧ��ԭ��: {reason}");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryFailedEvent(Id, AdId, reason));
    }

    /// <summary>
    /// ���Ϊ��ʱ
    /// </summary>
    public void MarkAsTimeout()
    {
        Status = DeliveryStatus.Timeout;
        BillingDetails = BillingDetails.AddNote("Ͷ�ų�ʱ");

        UpdateLastModifiedTime();
        AddDomainEvent(new DeliveryTimeoutEvent(Id, AdId));
    }

    /// <summary>
    /// ��ȡͶ��Ч��
    /// </summary>
    public DeliveryPerformance GetPerformance()
    {
        return new DeliveryPerformance(
            hasClick: ClickTime.HasValue,
            hasConversion: ConversionTime.HasValue,
            totalCost: Cost,
            revenue: Revenue ?? 0,
            qualityScore: QualityScore,
            clickLatency: ClickTime.HasValue ? (ClickTime.Value - Timestamp).TotalMilliseconds : null,
            conversionLatency: ConversionTime.HasValue ? (ConversionTime.Value - Timestamp).TotalMilliseconds : null
        );
    }

    /// <summary>
    /// �Ƿ���ЧͶ��
    /// </summary>
    public bool IsSuccessfulDelivery => Status == DeliveryStatus.Success;

    /// <summary>
    /// �Ƿ��е��
    /// </summary>
    public bool HasClick => ClickTime.HasValue;

    /// <summary>
    /// �Ƿ���ת��
    /// </summary>
    public bool HasConversion => ConversionTime.HasValue;

    /// <summary>
    /// ��ȡͶ��ʱ�������룩
    /// </summary>
    public double GetDeliveryDurationMs()
    {
        return (DateTime.UtcNow - Timestamp).TotalMilliseconds;
    }

    /// <summary>
    /// ��ȡ���ʱ�������룩
    /// </summary>
    public double? GetClickDurationMs()
    {
        return ClickTime.HasValue ? (ClickTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// ��ȡת��ʱ�������룩
    /// </summary>
    public double? GetConversionDurationMs()
    {
        return ConversionTime.HasValue ? (ConversionTime.Value - Timestamp).TotalMilliseconds : null;
    }

    /// <summary>
    /// ����ROI��Ͷ�ʻر��ʣ�
    /// </summary>
    public decimal? CalculateROI()
    {
        if (!Revenue.HasValue || Cost == 0)
            return null;

        return (Revenue.Value - Cost) / Cost;
    }

    /// <summary>
    /// ����ROAS�����֧���ر��ʣ�
    /// </summary>
    public decimal? CalculateROAS()
    {
        if (!Revenue.HasValue || Cost == 0)
            return null;

        return Revenue.Value / Cost;
    }

    /// <summary>
    /// ��ȡ�ض����͵���������Ϣ
    /// </summary>
    /// <typeparam name="T">����������</typeparam>
    /// <param name="contextType">���������ͱ�ʶ</param>
    /// <returns>ָ�����͵���������Ϣ������������򷵻�null</returns>
    public T? GetContext<T>(string contextType) where T : class, ITargetingContext
    {
        return DeliveryContexts.TryGetValue(contextType, out var context) ? context as T : null;
    }

    /// <summary>
    /// ����Ƿ�����ض����͵�������
    /// </summary>
    /// <param name="contextType">���������ͱ�ʶ</param>
    /// <returns>�Ƿ����ָ�����͵�������</returns>
    public bool HasContext(string contextType)
    {
        return DeliveryContexts.ContainsKey(contextType);
    }

    /// <summary>
    /// ��ȡ��������������
    /// </summary>
    /// <returns>�������������͵ļ���</returns>
    public IReadOnlyCollection<string> GetContextTypes()
    {
        return DeliveryContexts.Keys.ToList().AsReadOnly();
    }

    /// <summary>
    /// ���������л�ȡ�û���ʶ��
    /// </summary>
    /// <returns>�û���ʶ��������������򷵻�null</returns>
    public string? GetUserId()
    {
        var userContext = GetContext<ITargetingContext>("User");
        return userContext?.GetPropertyValue<string>("UserId");
    }

    /// <summary>
    /// ���������л�ȡ�豸����
    /// </summary>
    /// <returns>�豸���ͣ�����������򷵻�null</returns>
    public string? GetDeviceType()
    {
        var deviceContext = GetContext<ITargetingContext>("Device");
        return deviceContext?.GetPropertyValue<string>("DeviceType");
    }

    /// <summary>
    /// ���������л�ȡ����λ����Ϣ
    /// </summary>
    /// <returns>����λ����Ϣ������������򷵻�null</returns>
    public string? GetGeoLocation()
    {
        var geoContext = GetContext<ITargetingContext>("Geo");
        return geoContext?.GetPropertyValue<string>("Country") ?? geoContext?.GetPropertyValue<string>("Region");
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(Guid adId, Guid campaignId, Guid mediaResourceId, Guid placementId,
        string impressionId, string requestId, decimal price, decimal cost, decimal bidPrice,
        IReadOnlyDictionary<string, ITargetingContext> deliveryContexts, BillingDetails billingDetails)
    {
        if (adId == Guid.Empty)
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        if (campaignId == Guid.Empty)
            throw new ArgumentException("�ID����Ϊ��", nameof(campaignId));

        if (mediaResourceId == Guid.Empty)
            throw new ArgumentException("ý����ԴID����Ϊ��", nameof(mediaResourceId));

        if (placementId == Guid.Empty)
            throw new ArgumentException("���λID����Ϊ��", nameof(placementId));

        if (string.IsNullOrWhiteSpace(impressionId))
            throw new ArgumentException("չʾID����Ϊ��", nameof(impressionId));

        if (string.IsNullOrWhiteSpace(requestId))
            throw new ArgumentException("����ID����Ϊ��", nameof(requestId));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "�۸���Ϊ����");

        if (cost < 0)
            throw new ArgumentOutOfRangeException(nameof(cost), "�ɱ�����Ϊ����");

        if (bidPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(bidPrice), "���ۼ۸���Ϊ����");

        ArgumentNullException.ThrowIfNull(deliveryContexts);
        ArgumentNullException.ThrowIfNull(billingDetails);
    }
}

