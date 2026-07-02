using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Enums;
using AdPulse.Core.Shared.Constants;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// Ԥ����Ϣֵ����
/// </summary>
public class BudgetInfo : ValueObject
{
    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal TotalBudget { get; private set; }

    /// <summary>
    /// ��Ԥ�㣨�֣�
    /// </summary>
    public decimal DailyBudget { get; private set; }

    /// <summary>
    /// �����ѽ��֣�
    /// </summary>
    public decimal SpentAmount { get; private set; }

    /// <summary>
    /// ʣ��Ԥ�㣨�֣�
    /// </summary>
    public decimal RemainingBudget => TotalBudget - SpentAmount;

    /// <summary>
    /// Ԥ������
    /// </summary>
    public BudgetType Type { get; private set; }

    /// <summary>
    /// ��Ч��ʼʱ��
    /// </summary>
    public DateTime ValidFrom { get; private set; }

    /// <summary>
    /// ��Ч����ʱ��
    /// </summary>
    public DateTime ValidTo { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private BudgetInfo() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public BudgetInfo(
        decimal totalBudget,
        decimal dailyBudget,
        BudgetType type = BudgetType.Standard,
        DateTime? validFrom = null,
        DateTime? validTo = null)
    {
        ValidateInputs(totalBudget, dailyBudget);

        TotalBudget = totalBudget;
        DailyBudget = dailyBudget;
        SpentAmount = 0m;
        Type = type;
        ValidFrom = validFrom ?? DateTime.UtcNow;
        ValidTo = validTo ?? DateTime.MaxValue;
    }

    /// <summary>
    /// ������׼Ԥ��
    /// </summary>
    public static BudgetInfo CreateStandard(decimal totalBudget, decimal dailyBudget)
    {
        return new BudgetInfo(totalBudget, dailyBudget, BudgetType.Standard);
    }

    /// <summary>
    /// ��������Ԥ��
    /// </summary>
    public static BudgetInfo CreateUnlimited(decimal dailyBudget)
    {
        return new BudgetInfo(decimal.MaxValue, dailyBudget, BudgetType.Unlimited);
    }

    /// <summary>
    /// ��������Ԥ��
    /// </summary>
    public static BudgetInfo CreateTrial(decimal totalBudget, DateTime validTo)
    {
        var dailyBudget = Math.Min(totalBudget, DefaultValues.Budget.DefaultDailyBudget * 100);
        return new BudgetInfo(totalBudget, dailyBudget, BudgetType.Trial, DateTime.UtcNow, validTo);
    }

    /// <summary>
    /// ��ȡʣ����Ԥ��
    /// </summary>
    public decimal GetRemainingDaily()
    {
        // ����򻯴����ʵ��Ӧ�ø��ݵ��������Ѽ���
        return Math.Min(DailyBudget, RemainingBudget);
    }

    /// <summary>
    /// �Ƿ�Ԥ��ľ�
    /// </summary>
    public bool IsExhausted()
    {
        return RemainingBudget <= 0 || DateTime.UtcNow > ValidTo;
    }

    /// <summary>
    /// �Ƿ��������ָ�����
    /// </summary>
    public bool CanSpend(decimal amount)
    {
        if (amount <= 0)
            return false;

        if (IsExhausted())
            return false;

        return RemainingBudget >= amount;
    }

    // Replace the 'with' expression with a method to create a new instance
    public BudgetInfo RecordSpending(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("���ѽ��������0", nameof(amount));

        if (!CanSpend(amount))
            throw new InvalidOperationException("Ԥ�㲻�㣬�޷���¼����");

        return new BudgetInfo(TotalBudget, DailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = SpentAmount + amount
        };
    }

    /// <summary>
    /// ��ȡ������
    /// </summary>
    public double GetSpendingRate()
    {
        if (TotalBudget <= 0)
            return 0;

        return (double)(SpentAmount / TotalBudget);
    }

    /// <summary>
    /// ����Ԥ��
    /// </summary>
    public BudgetInfo UpdateBudget(decimal newTotalBudget, decimal newDailyBudget)
    {
        ValidateInputs(newTotalBudget, newDailyBudget);

        if (newTotalBudget < SpentAmount)
            throw new ArgumentException("�µ���Ԥ�㲻��С�������ѽ��", nameof(newTotalBudget));

        return new BudgetInfo(newTotalBudget, newDailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = SpentAmount
        };
    }

    /// <summary>
    /// ��������
    /// </summary>
    public BudgetInfo ResetSpending()
    {
        return new BudgetInfo(TotalBudget, DailyBudget, Type, ValidFrom, ValidTo)
        {
            SpentAmount = 0m
        };
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(decimal totalBudget, decimal dailyBudget)
    {
        if (totalBudget <= 0)
            throw new ArgumentException("��Ԥ��������0", nameof(totalBudget));

        if (dailyBudget <= 0)
            throw new ArgumentException("��Ԥ��������0", nameof(dailyBudget));

        if (dailyBudget > totalBudget && totalBudget != decimal.MaxValue)
            throw new ArgumentException("��Ԥ�㲻�ܳ�����Ԥ��", nameof(dailyBudget));
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalBudget;
        yield return DailyBudget;
        yield return SpentAmount;
        yield return Type;
        yield return ValidFrom;
        yield return ValidTo;
    }
}

