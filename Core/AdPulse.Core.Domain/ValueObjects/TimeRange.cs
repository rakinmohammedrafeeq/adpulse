using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// �����ʱ�䷶Χ��ʹ��DateTime��ʾ��
/// ���ڱ�ʾ������ȷ��ʼ�ͽ���ʱ���ʱ�䷶Χ����ʱ�䡢����ʱ���
/// </summary>
public class TimeRange : ValueObject
{
    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime StartTime { get; private set; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime EndTime { get; private set; }

    /// <summary>
    /// ʱ�䷶Χ��С
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TimeRange() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TimeRange(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("��ʼʱ��������ڽ���ʱ��");

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// ����ʱ�䷶Χ
    /// </summary>
    public static TimeRange Create(DateTime startTime, DateTime endTime)
    {
        return new TimeRange(startTime, endTime);
    }

    /// <summary>
    /// ���������ڿ�ʼ��ʱ�䷶Χ
    /// </summary>
    public static TimeRange CreateFromNow(TimeSpan duration)
    {
        var now = DateTime.UtcNow;
        return new TimeRange(now, now.Add(duration));
    }

    /// <summary>
    /// ����ָ��ʱ����ʱ�䷶Χ
    /// </summary>
    public static TimeRange CreateWithDuration(DateTime startTime, TimeSpan duration)
    {
        return new TimeRange(startTime, startTime.Add(duration));
    }

    /// <summary>
    /// ���������ʱ�䷶Χ
    /// </summary>
    public static TimeRange CreateToday()
    {
        var today = DateTime.Today;
        return new TimeRange(today, today.AddDays(1).AddTicks(-1));
    }

    /// <summary>
    /// �������ܵ�ʱ�䷶Χ
    /// </summary>
    public static TimeRange CreateThisWeek()
    {
        var today = DateTime.Today;
        var daysToSubtract = (int)today.DayOfWeek;
        var startOfWeek = today.AddDays(-daysToSubtract);
        var endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);
        return new TimeRange(startOfWeek, endOfWeek);
    }

    /// <summary>
    /// ���ָ��ʱ���Ƿ��ڷ�Χ��
    /// </summary>
    public bool Contains(DateTime time)
    {
        return time >= StartTime && time <= EndTime;
    }

    /// <summary>
    /// ����Ƿ�����һ��ʱ�䷶Χ�ص�
    /// </summary>
    public bool OverlapsWith(TimeRange other)
    {
        return StartTime <= other.EndTime && EndTime >= other.StartTime;
    }

    /// <summary>
    /// ��ȡ����һ��ʱ�䷶Χ�Ľ���
    /// </summary>
    public TimeRange? GetIntersection(TimeRange other)
    {
        if (!OverlapsWith(other))
            return null;

        var intersectionStart = StartTime > other.StartTime ? StartTime : other.StartTime;
        var intersectionEnd = EndTime < other.EndTime ? EndTime : other.EndTime;

        return new TimeRange(intersectionStart, intersectionEnd);
    }

    /// <summary>
    /// ��չʱ�䷶Χ
    /// </summary>
    public TimeRange Extend(TimeSpan beforeStart, TimeSpan afterEnd)
    {
        return new TimeRange(StartTime.Subtract(beforeStart), EndTime.Add(afterEnd));
    }

    /// <summary>
    /// ����Ƿ������ǰʱ��
    /// </summary>
    public bool ContainsNow()
    {
        return Contains(DateTime.UtcNow);
    }

    /// <summary>
    /// ����Ƿ��ѹ���
    /// </summary>
    public bool IsExpired()
    {
        return EndTime < DateTime.UtcNow;
    }

    /// <summary>
    /// ����Ƿ���δ��ʼ
    /// </summary>
    public bool IsNotStarted()
    {
        return StartTime > DateTime.UtcNow;
    }

    /// <summary>
    /// ����Ƿ����ڽ�����
    /// </summary>
    public bool IsActive()
    {
        return ContainsNow();
    }

    /// <summary>
    /// �ָ�ʱ�䷶ΧΪָ��ʱ���Ķ�
    /// </summary>
    public IEnumerable<TimeRange> Split(TimeSpan segmentDuration)
    {
        if (segmentDuration <= TimeSpan.Zero)
            throw new ArgumentException("�ֶ�ʱ���������0", nameof(segmentDuration));

        var current = StartTime;
        while (current < EndTime)
        {
            var segmentEnd = current.Add(segmentDuration);
            if (segmentEnd > EndTime)
                segmentEnd = EndTime;

            yield return new TimeRange(current, segmentEnd);
            current = segmentEnd;
        }
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    /// <summary>
    /// �ַ�����ʾ
    /// </summary>
    public override string ToString()
    {
        return $"{StartTime:yyyy-MM-dd HH:mm:ss} - {EndTime:yyyy-MM-dd HH:mm:ss} ({Duration})";
    }
}