using AdPulse.Core.Domain.Common;
using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Shared.Constants;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// Ͷ�Ų���ֵ����
/// </summary>
public class DeliveryPolicy : ValueObject
{
    /// <summary>
    /// Ͷ��ģʽ
    /// </summary>
    public DeliveryMode DeliveryMode { get; private set; }

    /// <summary>
    /// ���۲���
    /// </summary>
    public BiddingStrategy BiddingStrategy { get; private set; }

    /// <summary>
    /// �������ۼ۸񣨷֣�
    /// </summary>
    public decimal BaseBidPrice { get; private set; }

    /// <summary>
    /// �����ۼ۸񣨷֣�
    /// </summary>
    public decimal MaxBidPrice { get; private set; }

    /// <summary>
    /// ÿ��Ƶ������
    /// </summary>
    public int FrequencyCapDaily { get; private set; }

    /// <summary>
    /// ÿСʱƵ������
    /// </summary>
    public int FrequencyCapHourly { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// Ͷ�ſ�ʼʱ��
    /// </summary>
    public DateTime? StartTime { get; private set; }

    /// <summary>
    /// Ͷ�Ž���ʱ��
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// ʱ��
    /// </summary>
    public string? TimeZone { get; private set; }

    /// <summary>
    /// ���۵�������
    /// </summary>
    public decimal BidAdjustmentFactor { get; private set; } = 1.0m;

    /// <summary>
    /// �Ƿ��������ܳ���
    /// </summary>
    public bool EnableSmartBidding { get; private set; } = false;

    /// <summary>
    /// Ͷ�����ȼ���1-10������Խ�����ȼ�Խ�ߣ�
    /// </summary>
    public int Priority { get; private set; } = 5;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DeliveryPolicy() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public DeliveryPolicy(
        DeliveryMode deliveryMode,
        BiddingStrategy biddingStrategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        int frequencyCapDaily = DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
        int frequencyCapHourly = DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
        decimal dailyBudget = DefaultValues.Budget.DefaultDailyBudget * 100, // ת��Ϊ��
        decimal totalBudget = DefaultValues.Budget.DefaultTotalBudget * 100, // ת��Ϊ��
        DateTime? startTime = null,
        DateTime? endTime = null,
        string? timeZone = null,
        decimal bidAdjustmentFactor = 1.0m,
        bool enableSmartBidding = false,
        int priority = 5)
    {
        ValidateInputs(baseBidPrice, maxBidPrice, frequencyCapDaily, frequencyCapHourly, 
                      dailyBudget, totalBudget, priority);

        DeliveryMode = deliveryMode;
        BiddingStrategy = biddingStrategy;
        BaseBidPrice = baseBidPrice;
        MaxBidPrice = maxBidPrice;
        FrequencyCapDaily = frequencyCapDaily;
        FrequencyCapHourly = frequencyCapHourly;
        DailyBudget = dailyBudget;
        TotalBudget = totalBudget;
        StartTime = startTime;
        EndTime = endTime;
        TimeZone = timeZone;
        BidAdjustmentFactor = bidAdjustmentFactor;
        EnableSmartBidding = enableSmartBidding;
        Priority = priority;
    }

    /// <summary>
    /// ��������Ͷ�Ų���
    /// </summary>
    public static DeliveryPolicy CreateGuaranteed(
        decimal baseBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        DateTime? startTime = null,
        DateTime? endTime = null)
    {
        return new DeliveryPolicy(
            DeliveryMode.Guaranteed,
            BiddingStrategy.FixedBid,
            baseBidPrice,
            baseBidPrice, // ����Ͷ��ʱ�����۵��ڻ�������
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            startTime,
            endTime,
            priority: 8 // ����Ͷ�����ȼ��ϸ�
        );
    }

    /// <summary>
    /// ��������Ͷ�Ų���
    /// </summary>
    public static DeliveryPolicy CreateBidding(
        BiddingStrategy biddingStrategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        bool enableSmartBidding = true)
    {
        return new DeliveryPolicy(
            DeliveryMode.Bidding,
            biddingStrategy,
            baseBidPrice,
            maxBidPrice,
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            enableSmartBidding: enableSmartBidding,
            priority: 5 // ����Ͷ��Ĭ�����ȼ�
        );
    }

    /// <summary>
    /// �������Ͷ�Ų���
    /// </summary>
    public static DeliveryPolicy CreateMixed(
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal dailyBudget,
        decimal totalBudget,
        int guaranteedPriority = 7)
    {
        return new DeliveryPolicy(
            DeliveryMode.Mixed,
            BiddingStrategy.AutoBid,
            baseBidPrice,
            maxBidPrice,
            DefaultValues.FrequencyControl.DefaultDailyFrequencyCap,
            DefaultValues.FrequencyControl.DefaultHourlyFrequencyCap,
            dailyBudget,
            totalBudget,
            enableSmartBidding: true,
            priority: guaranteedPriority
        );
    }

    /// <summary>
    /// ����ʵ�ʳ���
    /// </summary>
    public decimal CalculateActualBid(decimal qualityScore = 1.0m, decimal marketPrice = 0m)
    {
        decimal adjustedBaseBid = BaseBidPrice * BidAdjustmentFactor * qualityScore;

        return BiddingStrategy switch
        {
            BiddingStrategy.FixedBid => Math.Min(adjustedBaseBid, MaxBidPrice),
            BiddingStrategy.AutoBid => CalculateAutoBid(adjustedBaseBid, marketPrice),
            BiddingStrategy.TargetCPA => CalculateTargetCPABid(adjustedBaseBid),
            BiddingStrategy.TargetROAS => CalculateTargetROASBid(adjustedBaseBid),
            BiddingStrategy.MaximizeClicks => CalculateMaximizeClicksBid(adjustedBaseBid),
            BiddingStrategy.MaximizeConversions => CalculateMaximizeConversionsBid(adjustedBaseBid),
            _ => Math.Min(adjustedBaseBid, MaxBidPrice)
        };
    }

    /// <summary>
    /// �ж��Ƿ���Ͷ��ʱ�䷶Χ��
    /// </summary>
    public bool IsWithinDeliveryTime(DateTime currentTime)
    {
        var targetTime = TimeZone != null ? 
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentTime, TimeZone) : 
            currentTime;

        if (StartTime.HasValue && targetTime < StartTime.Value)
            return false;

        if (EndTime.HasValue && targetTime > EndTime.Value)
            return false;

        return true;
    }

    /// <summary>
    /// �ж��Ƿ񳬳�Ƶ������
    /// </summary>
    public bool IsFrequencyCapExceeded(int dailyImpressions, int hourlyImpressions)
    {
        return dailyImpressions >= FrequencyCapDaily || hourlyImpressions >= FrequencyCapHourly;
    }

    /// <summary>
    /// �ж��Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal spentDailyBudget, decimal spentTotalBudget, decimal bidPrice)
    {
        return (spentDailyBudget + bidPrice) <= DailyBudget && 
               (spentTotalBudget + bidPrice) <= TotalBudget;
    }

    /// <summary>
    /// ��ȡͶ��Ȩ�أ��������ȼ���ģʽ��
    /// </summary>
    public decimal GetDeliveryWeight()
    {
        var basePriority = Priority / 10.0m;
        
        return DeliveryMode switch
        {
            DeliveryMode.Guaranteed => basePriority * 1.5m, // ����Ͷ��Ȩ�ظ���
            DeliveryMode.Priority => basePriority * 1.3m,   // ���ȼ�Ͷ��Ȩ�ؽϸ�
            DeliveryMode.Bidding => basePriority * 1.0m,    // ����Ͷ�ű�׼Ȩ��
            DeliveryMode.Mixed => basePriority * 1.2m,      // ���Ͷ��Ȩ������
            _ => basePriority
        };
    }

    /// <summary>
    /// �Զ����ۼ���
    /// </summary>
    private decimal CalculateAutoBid(decimal baseBid, decimal marketPrice)
    {
        if (marketPrice > 0)
        {
            // �����г��۸�Ķ�̬����
            var adjustedBid = Math.Min(baseBid, marketPrice * 1.1m);
            return Math.Min(adjustedBid, MaxBidPrice);
        }
        
        return Math.Min(baseBid, MaxBidPrice);
    }

    /// <summary>
    /// Ŀ��CPA���ۼ���
    /// </summary>
    private decimal CalculateTargetCPABid(decimal baseBid)
    {
        // �򻯵�Ŀ��CPA���㣬ʵ��ʵ����Ҫ������ʷת������
        return Math.Min(baseBid * 0.8m, MaxBidPrice);
    }

    /// <summary>
    /// Ŀ��ROAS���ۼ���
    /// </summary>
    private decimal CalculateTargetROASBid(decimal baseBid)
    {
        // �򻯵�Ŀ��ROAS���㣬ʵ��ʵ����Ҫ��������Ŀ��
        return Math.Min(baseBid * 0.9m, MaxBidPrice);
    }

    /// <summary>
    /// ��󻯵�����ۼ���
    /// </summary>
    private decimal CalculateMaximizeClicksBid(decimal baseBid)
    {
        // ��Ԥ��Լ������󻯵����
        return Math.Min(baseBid * 1.1m, MaxBidPrice);
    }

    /// <summary>
    /// ���ת�����ۼ���
    /// </summary>
    private decimal CalculateMaximizeConversionsBid(decimal baseBid)
    {
        // ��Ԥ��Լ�������ת����
        return Math.Min(baseBid * 1.2m, MaxBidPrice);
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(decimal baseBidPrice, decimal maxBidPrice, 
        int frequencyCapDaily, int frequencyCapHourly, decimal dailyBudget, 
        decimal totalBudget, int priority)
    {
        if (baseBidPrice < ValidationConstants.NumberRange.MinBidPrice * 100)
            throw new ArgumentOutOfRangeException(nameof(baseBidPrice), "�������۲��ܵ�����Сֵ");

        if (maxBidPrice < baseBidPrice)
            throw new ArgumentException("�����۲��ܵ��ڻ�������");

        if (maxBidPrice > ValidationConstants.NumberRange.MaxBidPrice * 100)
            throw new ArgumentOutOfRangeException(nameof(maxBidPrice), "�����۲��ܳ������ֵ");

        if (frequencyCapDaily <= 0 || frequencyCapHourly <= 0)
            throw new ArgumentOutOfRangeException("Ƶ�����ޱ������0");

        if (frequencyCapHourly > frequencyCapDaily)
            throw new ArgumentException("ÿСʱƵ�����޲��ܳ���ÿ��Ƶ������");

        if (dailyBudget <= 0 || totalBudget <= 0)
            throw new ArgumentOutOfRangeException("Ԥ��������0");

        if (dailyBudget > totalBudget)
            throw new ArgumentException("��Ԥ�㲻�ܳ�����Ԥ��");

        if (priority < 1 || priority > 10)
            throw new ArgumentOutOfRangeException(nameof(priority), "���ȼ�������1-10֮��");
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeliveryMode;
        yield return BiddingStrategy;
        yield return BaseBidPrice;
        yield return MaxBidPrice;
        yield return FrequencyCapDaily;
        yield return FrequencyCapHourly;
        yield return DailyBudget;
        yield return TotalBudget;
        yield return StartTime ?? DateTime.MinValue;
        yield return EndTime ?? DateTime.MaxValue;
        yield return TimeZone ?? string.Empty;
        yield return BidAdjustmentFactor;
        yield return EnableSmartBidding;
        yield return Priority;
    }
}