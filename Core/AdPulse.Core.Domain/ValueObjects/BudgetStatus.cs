using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// Ԥ��״ֵ̬����
/// </summary>
public class BudgetStatus : ValueObject
{
    /// <summary>
    /// �ID
    /// </summary>
    public Guid CampaignId { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// ������Ԥ�㣨�֣�
    /// </summary>
    public decimal SpentBudget { get; private set; }

    /// <summary>
    /// ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentBudget;

    /// <summary>
    /// ����������Ԥ�㣨�֣�
    /// </summary>
    public decimal TodaySpent { get; private set; }

    /// <summary>
    /// ����ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TodayRemaining => DailyBudget - TodaySpent;

    /// <summary>
    /// Ԥ��״̬
    /// </summary>
    public BudgetStatusType Status { get; private set; }

    /// <summary>
    /// �����ٶȣ���/Сʱ��
    /// </summary>
    public decimal? BurnRate { get; private set; }

    /// <summary>
    /// Ԥ�ƺľ�ʱ��
    /// </summary>
    public DateTime? EstimatedExhaustionTime { get; private set; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BudgetStatus()
    {
        CampaignId = Guid.Empty;
        TotalBudget = 0;
        DailyBudget = 0;
        SpentBudget = 0;
        TodaySpent = 0;
        Status = BudgetStatusType.Active;
        LastUpdated = DateTime.UtcNow;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public BudgetStatus(
        Guid campaignId,
        decimal totalBudget,
        decimal dailyBudget,
        decimal spentBudget = 0,
        decimal todaySpent = 0,
        BudgetStatusType status = BudgetStatusType.Active,
        decimal? burnRate = null,
        DateTime? estimatedExhaustionTime = null,
        DateTime? lastUpdated = null)
    {
        ValidateInput(campaignId, totalBudget, dailyBudget, spentBudget, todaySpent);

        CampaignId = campaignId;
        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        SpentBudget = spentBudget;
        TodaySpent = todaySpent;
        Status = status;
        BurnRate = burnRate;
        EstimatedExhaustionTime = estimatedExhaustionTime;
        LastUpdated = lastUpdated ?? DateTime.UtcNow;
    }

    /// <summary>
    /// ����Ԥ��״̬
    /// </summary>
    public static BudgetStatus Create(
        Guid campaignId,
        decimal totalBudget,
        decimal dailyBudget)
    {
        return new BudgetStatus(campaignId, totalBudget, dailyBudget);
    }

    /// <summary>
    /// �Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal amount)
    {
        return Status == BudgetStatusType.Active &&
               RemainingBudget >= amount &&
               TodayRemaining >= amount;
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public BudgetStatus SpendBudget(decimal amount, bool isToday = true)
    {
        if (amount < 0)
            throw new ArgumentException("���ѽ���Ϊ����", nameof(amount));

        if (!HasSufficientBudget(amount))
            throw new InvalidOperationException("Ԥ�㲻��");

        var newSpentBudget = SpentBudget + amount;
        var newTodaySpent = isToday ? TodaySpent + amount : TodaySpent;

        // �Զ�����״̬
        var newStatus = Status;
        if (newSpentBudget >= TotalBudget)
            newStatus = BudgetStatusType.Exhausted;
        else if (newTodaySpent >= DailyBudget)
            newStatus = BudgetStatusType.OverLimit;

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            newSpentBudget,
            newTodaySpent,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ���������ٶ�
    /// </summary>
    public BudgetStatus UpdateBurnRate(decimal burnRate)
    {
        if (burnRate < 0)
            throw new ArgumentException("�����ٶȲ���Ϊ����", nameof(burnRate));

        // ����Ԥ�ƺľ�ʱ��
        DateTime? estimatedExhaustionTime = null;
        if (burnRate > 0 && RemainingBudget > 0)
        {
            var hoursToExhaustion = (double)(RemainingBudget / burnRate);
            estimatedExhaustionTime = DateTime.UtcNow.AddHours(hoursToExhaustion);
        }

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            Status,
            burnRate,
            estimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ���ý�������
    /// </summary>
    public BudgetStatus ResetDailySpent()
    {
        var newStatus = Status == BudgetStatusType.OverLimit ? BudgetStatusType.Active : Status;

        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            0,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public BudgetStatus UpdateStatus(BudgetStatusType status)
    {
        return new BudgetStatus(
            CampaignId,
            TotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            status,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public BudgetStatus AddBudget(decimal additionalBudget)
    {
        if (additionalBudget < 0)
            throw new ArgumentException("׷��Ԥ�㲻��Ϊ����", nameof(additionalBudget));

        var newTotalBudget = TotalBudget + additionalBudget;
        var newStatus = Status == BudgetStatusType.Exhausted ? BudgetStatusType.Active : Status;

        return new BudgetStatus(
            CampaignId,
            newTotalBudget,
            DailyBudget,
            SpentBudget,
            TodaySpent,
            newStatus,
            BurnRate,
            EstimatedExhaustionTime,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Ԥ��ʹ����
    /// </summary>
    public decimal UsageRate => TotalBudget > 0 ? SpentBudget / TotalBudget : 0;

    /// <summary>
    /// ����Ԥ��ʹ����
    /// </summary>
    public decimal DailyUsageRate => DailyBudget > 0 ? TodaySpent / DailyBudget : 0;

    /// <summary>
    /// �Ƿ�Ԥ��ľ�
    /// </summary>
    public bool IsExhausted => Status == BudgetStatusType.Exhausted;

    /// <summary>
    /// �Ƿ����Ԥ��ľ�
    /// </summary>
    public bool IsDailyExhausted => Status == BudgetStatusType.OverLimit;

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CampaignId;
        yield return TotalBudget;
        yield return DailyBudget;
        yield return SpentBudget;
        yield return TodaySpent;
        yield return Status;
        yield return BurnRate ?? 0m;
        yield return EstimatedExhaustionTime ?? DateTime.MinValue;
        yield return LastUpdated;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        Guid campaignId,
        decimal totalBudget,
        decimal dailyBudget,
        decimal spentBudget,
        decimal todaySpent)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("�ID����Ϊ��", nameof(campaignId));

        if (totalBudget < 0)
            throw new ArgumentException("��Ԥ�㲻��Ϊ����", nameof(totalBudget));

        if (dailyBudget < 0)
            throw new ArgumentException("��Ԥ�㲻��Ϊ����", nameof(dailyBudget));

        if (spentBudget < 0)
            throw new ArgumentException("������Ԥ�㲻��Ϊ����", nameof(spentBudget));

        if (todaySpent < 0)
            throw new ArgumentException("����������Ԥ�㲻��Ϊ����", nameof(todaySpent));

        if (spentBudget > totalBudget)
            throw new ArgumentException("������Ԥ�㲻�ܳ�����Ԥ��", nameof(spentBudget));

        if (todaySpent > dailyBudget)
            throw new ArgumentException("����������Ԥ�㲻�ܳ�����Ԥ��", nameof(todaySpent));
    }
}