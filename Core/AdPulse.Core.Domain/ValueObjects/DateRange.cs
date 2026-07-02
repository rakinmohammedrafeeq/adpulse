using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ���ڷ�Χ��ʹ��DateOnly��ʾ��
/// ���ڱ�ʾ���ڷ�Χ������������ʱ����Ϣ
/// </summary>
public class DateRange : ValueObject
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private DateRange()
    {
        StartDate = DateOnly.MinValue;
        EndDate = DateOnly.MinValue;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        ValidateInput(startDate, endDate);

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// �������ڷ�Χ
    /// </summary>
    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// ������������ڷ�Χ
    /// </summary>
    public static DateRange CreateToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return new DateRange(today, today);
    }

    /// <summary>
    /// �������ܵ����ڷ�Χ
    /// </summary>
    public static DateRange CreateThisWeek()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var daysToSubtract = (int)today.DayOfWeek;
        var startOfWeek = today.AddDays(-daysToSubtract);
        var endOfWeek = startOfWeek.AddDays(6);
        return new DateRange(startOfWeek, endOfWeek);
    }

    /// <summary>
    /// �������µ����ڷ�Χ
    /// </summary>
    public static DateRange CreateThisMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startOfMonth = new DateOnly(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        return new DateRange(startOfMonth, endOfMonth);
    }

    /// <summary>
    /// ������������ڷ�Χ
    /// </summary>
    public static DateRange CreateThisYear()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var startOfYear = new DateOnly(today.Year, 1, 1);
        var endOfYear = new DateOnly(today.Year, 12, 31);
        return new DateRange(startOfYear, endOfYear);
    }

    /// <summary>
    /// ��������Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// ��ȡ���ڷ�Χ����
    /// </summary>
    public int DaysCount => EndDate.DayNumber - StartDate.DayNumber + 1;

    /// <summary>
    /// ��չ���ڷ�Χ
    /// </summary>
    public DateRange Extend(int startDays, int endDays)
    {
        return new DateRange(
            StartDate.AddDays(-startDays),
            EndDate.AddDays(endDays));
    }

    /// <summary>
    /// ����Ƿ�����һ�����ڷ�Χ�ص�
    /// </summary>
    public bool OverlapsWith(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    /// <summary>
    /// ��ȡ����һ�����ڷ�Χ�Ľ���
    /// </summary>
    public DateRange? GetIntersection(DateRange other)
    {
        if (!OverlapsWith(other))
            return null;

        var intersectionStart = StartDate > other.StartDate ? StartDate : other.StartDate;
        var intersectionEnd = EndDate < other.EndDate ? EndDate : other.EndDate;

        return new DateRange(intersectionStart, intersectionEnd);
    }

    /// <summary>
    /// ��ȡ���ڷ�Χ�ڵ���������
    /// </summary>
    public IEnumerable<DateOnly> GetDates()
    {
        var current = StartDate;
        while (current <= EndDate)
        {
            yield return current;
            current = current.AddDays(1);
        }
    }

    /// <summary>
    /// ����Ƿ��������
    /// </summary>
    public bool ContainsToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return Contains(today);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    /// <summary>
    /// �ַ�����ʾ
    /// </summary>
    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} - {EndDate:yyyy-MM-dd} ({DaysCount} ��)";
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("��ʼ���ڲ��ܴ��ڽ�������");
    }
}