using AdPulse.Core.Shared.Enums;
using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// Ƶ��״ֵ̬����
/// </summary>
public class FrequencyStatus : ValueObject
{
    /// <summary>
    /// �û�ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// ���ID
    /// </summary>
    public Guid AdId { get; private set; }

    /// <summary>
    /// ����չʾ����
    /// </summary>
    public int TodayImpressions { get; private set; }

    /// <summary>
    /// ��Сʱչʾ����
    /// </summary>
    public int HourlyImpressions { get; private set; }

    /// <summary>
    /// ��Ƶ������
    /// </summary>
    public int DailyFrequencyCap { get; private set; }

    /// <summary>
    /// СʱƵ������
    /// </summary>
    public int HourlyFrequencyCap { get; private set; }

    /// <summary>
    /// ȫ��չʾ����
    /// </summary>
    public int TotalImpressions { get; private set; }

    /// <summary>
    /// ���չʾʱ��
    /// </summary>
    public DateTime? LastImpressionTime { get; private set; }

    /// <summary>
    /// �Ƿ񳬳�Ƶ������
    /// </summary>
    public bool IsFrequencyCapExceeded =>
        TodayImpressions >= DailyFrequencyCap ||
        HourlyImpressions >= HourlyFrequencyCap;

    /// <summary>
    /// ��Ͷ�Ŵ���
    /// </summary>
    public int RemainingImpressions => Math.Min(
        Math.Max(0, DailyFrequencyCap - TodayImpressions),
        Math.Max(0, HourlyFrequencyCap - HourlyImpressions));

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private FrequencyStatus()
    {
        UserId = Guid.Empty;
        AdId = Guid.Empty;
        TodayImpressions = 0;
        HourlyImpressions = 0;
        DailyFrequencyCap = 0;
        HourlyFrequencyCap = 0;
        TotalImpressions = 0;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public FrequencyStatus(
        Guid userId,
        Guid adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap,
        int todayImpressions = 0,
        int hourlyImpressions = 0,
        int totalImpressions = 0,
        DateTime? lastImpressionTime = null)
    {
        ValidateInput(userId, adId, dailyFrequencyCap, hourlyFrequencyCap, todayImpressions, hourlyImpressions, totalImpressions);

        UserId = userId;
        AdId = adId;
        DailyFrequencyCap = dailyFrequencyCap;
        HourlyFrequencyCap = hourlyFrequencyCap;
        TodayImpressions = todayImpressions;
        HourlyImpressions = hourlyImpressions;
        TotalImpressions = totalImpressions;
        LastImpressionTime = lastImpressionTime;
    }

    /// <summary>
    /// ����Ƶ��״̬
    /// </summary>
    public static FrequencyStatus Create(
        Guid userId,
        Guid adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap)
    {
        return new FrequencyStatus(userId, adId, dailyFrequencyCap, hourlyFrequencyCap);
    }

    /// <summary>
    /// ��¼չʾ
    /// </summary>
    public FrequencyStatus RecordImpression(DateTime? impressionTime = null)
    {
        var currentTime = impressionTime ?? DateTime.UtcNow;
        var lastTime = LastImpressionTime ?? DateTime.MinValue;

        // �ж��Ƿ���Ҫ���ü�����
        var newTodayImpressions = TodayImpressions;
        var newHourlyImpressions = HourlyImpressions;

        if (lastTime.Date != currentTime.Date)
        {
            // ���������ռ���
            newTodayImpressions = 0;
            newHourlyImpressions = 0;
        }
        else if (lastTime.Hour != currentTime.Hour)
        {
            // ��Сʱ����Сʱ����
            newHourlyImpressions = 0;
        }

        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            newTodayImpressions + 1,
            newHourlyImpressions + 1,
            TotalImpressions + 1,
            currentTime);
    }

    /// <summary>
    /// ����Ƶ������
    /// </summary>
    public FrequencyStatus UpdateFrequencyCaps(int dailyFrequencyCap, int hourlyFrequencyCap)
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            dailyFrequencyCap,
            hourlyFrequencyCap,
            TodayImpressions,
            HourlyImpressions,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// ���ý��ռ���
    /// </summary>
    public FrequencyStatus ResetDailyCount()
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            0,
            0,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// ����Сʱ����
    /// </summary>
    public FrequencyStatus ResetHourlyCount()
    {
        return new FrequencyStatus(
            UserId,
            AdId,
            DailyFrequencyCap,
            HourlyFrequencyCap,
            TodayImpressions,
            0,
            TotalImpressions,
            LastImpressionTime);
    }

    /// <summary>
    /// �Ƿ����Ͷ��
    /// </summary>
    public bool CanServe => !IsFrequencyCapExceeded;

    /// <summary>
    /// ��Ƶ��ʹ����
    /// </summary>
    public decimal DailyUsageRate => DailyFrequencyCap > 0 ? (decimal)TodayImpressions / DailyFrequencyCap : 0;

    /// <summary>
    /// СʱƵ��ʹ����
    /// </summary>
    public decimal HourlyUsageRate => HourlyFrequencyCap > 0 ? (decimal)HourlyImpressions / HourlyFrequencyCap : 0;

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return AdId;
        yield return TodayImpressions;
        yield return HourlyImpressions;
        yield return DailyFrequencyCap;
        yield return HourlyFrequencyCap;
        yield return TotalImpressions;
        yield return LastImpressionTime ?? DateTime.MinValue;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        Guid userId,
        Guid adId,
        int dailyFrequencyCap,
        int hourlyFrequencyCap,
        int todayImpressions,
        int hourlyImpressions,
        int totalImpressions)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("�û�ID����Ϊ��", nameof(userId));

        if (adId == Guid.Empty)
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        if (dailyFrequencyCap < 0)
            throw new ArgumentException("��Ƶ�����޲���Ϊ����", nameof(dailyFrequencyCap));

        if (hourlyFrequencyCap < 0)
            throw new ArgumentException("СʱƵ�����޲���Ϊ����", nameof(hourlyFrequencyCap));

        if (todayImpressions < 0)
            throw new ArgumentException("����չʾ��������Ϊ����", nameof(todayImpressions));

        if (hourlyImpressions < 0)
            throw new ArgumentException("��Сʱչʾ��������Ϊ����", nameof(hourlyImpressions));

        if (totalImpressions < 0)
            throw new ArgumentException("ȫ��չʾ��������Ϊ����", nameof(totalImpressions));

        if (hourlyFrequencyCap > dailyFrequencyCap && dailyFrequencyCap > 0)
            throw new ArgumentException("СʱƵ�����޲��ܴ�����Ƶ������");
    }
}