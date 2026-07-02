namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// ���ü�����
/// </summary>
public static class ConfigurationKeys
{
    /// <summary>
    /// ���ݿ���������
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// �����ݿ������ַ���
        /// </summary>
        public const string MainConnectionString = "Database:Main:ConnectionString";

        /// <summary>
        /// ֻ�����ݿ������ַ���
        /// </summary>
        public const string ReadOnlyConnectionString = "Database:ReadOnly:ConnectionString";

        /// <summary>
        /// ���ݿ����ӳش�С
        /// </summary>
        public const string ConnectionPoolSize = "Database:ConnectionPoolSize";

        /// <summary>
        /// ���ݿ����ʱʱ��
        /// </summary>
        public const string CommandTimeout = "Database:CommandTimeout";
    }

    /// <summary>
    /// Redis��������
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// Redis�����ַ���
        /// </summary>
        public const string ConnectionString = "Redis:ConnectionString";

        /// <summary>
        /// Redis���ݿ���
        /// </summary>
        public const string Database = "Redis:Database";

        /// <summary>
        /// Redis���ӳش�С
        /// </summary>
        public const string PoolSize = "Redis:PoolSize";

        /// <summary>
        /// Redis��ǰ׺
        /// </summary>
        public const string KeyPrefix = "Redis:KeyPrefix";
    }

    /// <summary>
    /// ���Ͷ������
    /// </summary>
    public static class AdDelivery
    {
        /// <summary>
        /// �������ʱʱ��
        /// </summary>
        public const string RequestTimeout = "AdDelivery:RequestTimeout";

        /// <summary>
        /// ��󲢷�������
        /// </summary>
        public const string MaxConcurrentRequests = "AdDelivery:MaxConcurrentRequests";

        /// <summary>
        /// Ĭ��QPS����
        /// </summary>       
        public const string DefaultQpsLimit = "AdDelivery:DefaultQpsLimit";

        /// <summary>
        /// ��滺�����ʱ��
        /// </summary>
        public const string AdCacheExpiration = "AdDelivery:AdCacheExpiration";
    }

    /// <summary>
    /// ���۷�������
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// ���۳�ʱʱ��
        /// </summary>
        public const string BidTimeout = "Bidding:BidTimeout";

        /// <summary>
        /// ��;��ۼ۸�
        /// </summary>
        public const string MinBidPrice = "Bidding:MinBidPrice";

        /// <summary>
        /// ��߾��ۼ۸�
        /// </summary>
        public const string MaxBidPrice = "Bidding:MaxBidPrice";

        /// <summary>
        /// ���۱�����
        /// </summary>
        public const string BidProtectionPeriod = "Bidding:BidProtectionPeriod";
    }

    /// <summary>
    /// ��־����
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// ��־����
        /// </summary>
        public const string LogLevel = "Logging:LogLevel:Default";

        /// <summary>
        /// �ṹ����־����
        /// </summary>
        public const string StructuredLogging = "Logging:StructuredLogging";

        /// <summary>
        /// ��־�ļ�·��
        /// </summary>
        public const string LogFilePath = "Logging:File:Path";

        /// <summary>
        /// ��־��������
        /// </summary>
        public const string LogRetentionDays = "Logging:RetentionDays";
    }

    /// <summary>
    /// �������
    /// </summary>
    public static class Monitoring
    {
        /// <summary>
        /// �������˵�
        /// </summary>
        public const string HealthCheckEndpoint = "Monitoring:HealthCheck:Endpoint";

        /// <summary>
        /// ָ���ռ����
        /// </summary>
        public const string MetricsInterval = "Monitoring:Metrics:Interval";

        /// <summary>
        /// �澯��ֵ
        /// </summary>
        public const string AlertThreshold = "Monitoring:Alert:Threshold";

        /// <summary>
        /// ���ܼ�ؿ���
        /// </summary>
        public const string PerformanceMonitoring = "Monitoring:Performance:Enabled";
    }
}