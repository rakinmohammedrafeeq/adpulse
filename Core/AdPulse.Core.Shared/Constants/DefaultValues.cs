namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// ϵͳĬ��ֵ����
/// </summary>
public static class DefaultValues
{
    /// <summary>
    /// ������Ĭ��ֵ
    /// </summary>
    public static class Advertisement
    {
        /// <summary>
        /// Ĭ�Ϲ��Ȩ��
        /// </summary>
        public const int DefaultWeight = 100;

        /// <summary>
        /// Ĭ�������÷�
        /// </summary>
        public const decimal DefaultQualityScore = 1.0m;

        /// <summary>
        /// Ĭ�ϵ����
        /// </summary>
        public const decimal DefaultClickThroughRate = 0.01m;

        /// <summary>
        /// Ĭ��ת����
        /// </summary>
        public const decimal DefaultConversionRate = 0.001m;

        /// <summary>
        /// Ĭ�Ϲ����Ч�� (��)
        /// </summary>
        public const int DefaultValidityPeriodDays = 30;
    }

    /// <summary>
    /// �������Ĭ��ֵ
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// Ĭ�ϵ׼� (��)
        /// </summary>
        public const decimal DefaultFloorPrice = 1.0m;

        /// <summary>
        /// Ĭ�Ͼ��۱���
        /// </summary>
        public const decimal DefaultBidMultiplier = 1.0m;

        /// <summary>
        /// Ĭ�Ͼ������ȼ�
        /// </summary>
        public const int DefaultBidPriority = 5;

        /// <summary>
        /// Ĭ�Ͼ�����Ч�� (��)
        /// </summary>
        public const int DefaultBidValiditySeconds = 3600;
    }

    /// <summary>
    /// �������Ĭ��ֵ
    /// </summary>
    public static class Targeting
    {
        /// <summary>
        /// Ĭ�϶���Ȩ��
        /// </summary>
        public const decimal DefaultTargetingWeight = 1.0m;

        /// <summary>
        /// Ĭ��ƥ�����ֵ
        /// </summary>
        public const decimal DefaultMatchThreshold = 0.5m;

        /// <summary>
        /// Ĭ�϶���뾶 (����)
        /// </summary>
        public const int DefaultTargetingRadiusKm = 50;

        /// <summary>
        /// Ĭ�����䷶Χ����
        /// </summary>
        public const int DefaultMinAge = 18;

        /// <summary>
        /// Ĭ�����䷶Χ����
        /// </summary>
        public const int DefaultMaxAge = 65;
    }

    /// <summary>
    /// Ԥ�����Ĭ��ֵ
    /// </summary>
    public static class Budget
    {
        /// <summary>
        /// Ĭ����Ԥ�� (Ԫ)
        /// </summary>
        public const decimal DefaultDailyBudget = 100.0m;

        /// <summary>
        /// Ĭ����Ԥ�� (Ԫ)
        /// </summary>
        public const decimal DefaultTotalBudget = 1000.0m;

        /// <summary>
        /// Ĭ��Ԥ��������
        /// </summary>
        public const decimal DefaultBudgetAllocationRatio = 1.0m;

        /// <summary>
        /// Ĭ��Ԥ��Ԥ����ֵ
        /// </summary>
        public const decimal DefaultBudgetAlertThreshold = 0.8m;
    }

    /// <summary>
    /// Ƶ�ο���Ĭ��ֵ
    /// </summary>
    public static class FrequencyControl
    {
        /// <summary>
        /// Ĭ��ÿ��Ƶ������
        /// </summary>
        public const int DefaultDailyFrequencyCap = 10;

        /// <summary>
        /// Ĭ��ÿСʱƵ������
        /// </summary>
        public const int DefaultHourlyFrequencyCap = 3;

        /// <summary>
        /// Ĭ��Ƶ�ο��ƴ��� (Сʱ)
        /// </summary>
        public const int DefaultFrequencyWindowHours = 24;

        /// <summary>
        /// Ĭ��Ƶ��ͳ�ƾ��� (����)
        /// </summary>
        public const int DefaultFrequencyPrecisionMinutes = 5;
    }

    /// <summary>
    /// �������Ĭ��ֵ
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Ĭ�ϻ������ʱ�� (����)
        /// </summary>
        public const int DefaultExpirationMinutes = 60;

        /// <summary>
        /// Ĭ�ϻ���ˢ�¼�� (����)
        /// </summary>
        public const int DefaultRefreshIntervalMinutes = 30;

        /// <summary>
        /// Ĭ�ϻ����С���� (MB)
        /// </summary>
        public const int DefaultCacheSizeLimitMB = 1024;

        /// <summary>
        /// Ĭ�ϻ���ѹ����ֵ (KB)
        /// </summary>
        public const int DefaultCompressionThresholdKB = 1024;
    }
}