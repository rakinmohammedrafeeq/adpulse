using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// һ���ڵ�ʱ��Σ�ʹ��TimeOnly��ʾ��
/// ���ڱ�ʾÿ�յ�ʱ��Σ��繤��ʱ�䡢Ӫҵʱ���
/// </summary>
public class TimeSlot : ValueObject
{
    /// <summary>
    /// ��ʼʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// ����ʱ�䣨Сʱ:���ӣ�
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private TimeSlot()
    {
        StartTime = TimeOnly.MinValue;
        EndTime = TimeOnly.MinValue;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public TimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        ValidateInput(startTime, endTime);

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// ����ʱ���
    /// </summary>
    public static TimeSlot Create(TimeOnly startTime, TimeOnly endTime)
    {
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// ����ʱ��Σ�Сʱ�ͷ��ӣ�
    /// </summary>
    public static TimeSlot Create(int startHour, int startMinute, int endHour, int endMinute)
    {
        var startTime = new TimeOnly(startHour, startMinute);
        var endTime = new TimeOnly(endHour, endMinute);
        return new TimeSlot(startTime, endTime);
    }

    /// <summary>
    /// ��������ʱ���
    /// </summary>
    public static TimeSlot CreateWorkingHours()
    {
        return new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
    }

    /// <summary>
    /// ����ȫ��ʱ���
    /// </summary>
    public static TimeSlot CreateAllDay()
    {
        return new TimeSlot(TimeOnly.MinValue, TimeOnly.MaxValue);
    }

    /// <summary>
    /// ���ʱ���Ƿ���ʱ�����
    /// </summary>
    public bool Contains(TimeOnly time)
    {
        if (StartTime <= EndTime)
        {
            // ������
            return time >= StartTime && time <= EndTime;
        }
        else
        {
            // ����
            return time >= StartTime || time <= EndTime;
        }
    }

    /// <summary>
    /// ��ȡʱ��γ���ʱ��
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            if (StartTime <= EndTime)
            {
                return EndTime - StartTime;
            }
            else
            {
                // ����
                return (TimeOnly.MaxValue - StartTime) + (EndTime - TimeOnly.MinValue);
            }
        }
    }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    public bool IsCrossDay => StartTime > EndTime;

    /// <summary>
    /// ����һ��ʱ����Ƿ��ص�
    /// </summary>
    public bool OverlapsWith(TimeSlot other)
    {
        if (!IsCrossDay && !other.IsCrossDay)
        {
            // ��������
            return StartTime <= other.EndTime && EndTime >= other.StartTime;
        }
        else if (IsCrossDay && other.IsCrossDay)
        {
            // ������
            return true; // �����ʱ��������ص�
        }
        else
        {
            // һ�����죬һ��������
            var crossDay = IsCrossDay ? this : other;
            var nonCrossDay = IsCrossDay ? other : this;
            
            return nonCrossDay.StartTime >= crossDay.StartTime || 
                   nonCrossDay.EndTime <= crossDay.EndTime ||
                   (nonCrossDay.StartTime <= crossDay.EndTime);
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
        return $"{StartTime:HH:mm} - {EndTime:HH:mm}{(IsCrossDay ? " (����)" : "")}";
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(TimeOnly startTime, TimeOnly endTime)
    {
        // TimeOnly���ͱ����Ѿ���֤����Ч�ԣ�����������ҵ���߼���֤
        // ��������ʱ��Σ����Բ���Ҫ������֤
    }
}