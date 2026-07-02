using AdPulse.Core.Domain.Common;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ������Ϣֵ����
/// </summary>
public class BiddingInfo : ValueObject
{
    /// <summary>
    /// ���۲���
    /// </summary>
    public BiddingStrategy Strategy { get; private set; }

    /// <summary>
    /// �������ۣ��֣�
    /// </summary>
    public decimal BaseBidPrice { get; private set; }

    /// <summary>
    /// �����ۣ��֣�
    /// </summary>
    public decimal MaxBidPrice { get; private set; }

    /// <summary>
    /// ��ǰ���ۣ��֣�
    /// </summary>
    public decimal CurrentBidPrice { get; private set; }

    /// <summary>
    /// ���۵�������
    /// </summary>
    public decimal BidAdjustmentFactor { get; private set; }

    /// <summary>
    /// Ԥ����Ϣ
    /// </summary>
    public BudgetInfo? Budget { get; private set; }

    /// <summary>
    /// ���۱�ǩ
    /// </summary>
    public IReadOnlyList<string> BidTags { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BiddingInfo()
    {
        Strategy = BiddingStrategy.FixedBid;
        BaseBidPrice = 0;
        MaxBidPrice = 0;
        CurrentBidPrice = 0;
        BidAdjustmentFactor = 1.0m;
        BidTags = Array.Empty<string>();
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public BiddingInfo(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal currentBidPrice = 0,
        decimal bidAdjustmentFactor = 1.0m,
        BudgetInfo? budget = null,
        IReadOnlyList<string>? bidTags = null)
    {
        ValidateInput(strategy, baseBidPrice, maxBidPrice, bidAdjustmentFactor);

        Strategy = strategy;
        BaseBidPrice = baseBidPrice;
        MaxBidPrice = maxBidPrice;
        CurrentBidPrice = currentBidPrice;
        BidAdjustmentFactor = bidAdjustmentFactor;
        Budget = budget;
        BidTags = bidTags ?? Array.Empty<string>();
    }

    /// <summary>
    /// ����������Ϣ
    /// </summary>
    public static BiddingInfo Create(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        BudgetInfo? budget = null)
    {
        return new BiddingInfo(strategy, baseBidPrice, maxBidPrice, budget: budget);
    }

    /// <summary>
    /// ���µ�ǰ����
    /// </summary>
    public BiddingInfo UpdateCurrentBidPrice(decimal currentBidPrice)
    {
        if (currentBidPrice < 0)
            throw new ArgumentException("��ǰ���۲���Ϊ����", nameof(currentBidPrice));

        if (currentBidPrice > MaxBidPrice)
            throw new ArgumentException("��ǰ���۲��ܳ���������", nameof(currentBidPrice));

        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            currentBidPrice,
            BidAdjustmentFactor,
            Budget,
            BidTags);
    }

    /// <summary>
    /// ������������
    /// </summary>
    public BiddingInfo WithBidAdjustmentFactor(decimal adjustmentFactor)
    {
        if (adjustmentFactor <= 0)
            throw new ArgumentException("���۵������ӱ������0", nameof(adjustmentFactor));

        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            adjustmentFactor,
            Budget,
            BidTags);
    }

    /// <summary>
    /// ����Ԥ����Ϣ
    /// </summary>
    public BiddingInfo WithBudget(BudgetInfo budget)
    {
        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            BidAdjustmentFactor,
            budget,
            BidTags);
    }

    /// <summary>
    /// ��Ӿ��۱�ǩ
    /// </summary>
    public BiddingInfo WithBidTags(params string[] tags)
    {
        var newTags = BidTags.Concat(tags).Distinct().ToArray();
        return new BiddingInfo(
            Strategy,
            BaseBidPrice,
            MaxBidPrice,
            CurrentBidPrice,
            BidAdjustmentFactor,
            Budget,
            newTags);
    }

    /// <summary>
    /// �Ƿ����㹻Ԥ��
    /// </summary>
    public bool HasSufficientBudget(decimal requiredAmount)
    {
        return Budget?.RemainingBudget >= requiredAmount;
    }

    /// <summary>
    /// ���������ĳ���
    /// </summary>
    public decimal CalculateAdjustedBidPrice()
    {
        var adjustedPrice = BaseBidPrice * BidAdjustmentFactor;
        return Math.Min(adjustedPrice, MaxBidPrice);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Strategy;
        yield return BaseBidPrice;
        yield return MaxBidPrice;
        yield return CurrentBidPrice;
        yield return BidAdjustmentFactor;
        yield return Budget ?? new object();

        foreach (var tag in BidTags.OrderBy(x => x))
        {
            yield return tag;
        }
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        BiddingStrategy strategy,
        decimal baseBidPrice,
        decimal maxBidPrice,
        decimal bidAdjustmentFactor)
    {
        if (baseBidPrice < 0)
            throw new ArgumentException("�������۲���Ϊ����", nameof(baseBidPrice));

        if (maxBidPrice < 0)
            throw new ArgumentException("�����۲���Ϊ����", nameof(maxBidPrice));

        if (maxBidPrice < baseBidPrice)
            throw new ArgumentException("�����۲���С�ڻ�������", nameof(maxBidPrice));

        if (bidAdjustmentFactor <= 0)
            throw new ArgumentException("���۵������ӱ������0", nameof(bidAdjustmentFactor));
    }
}