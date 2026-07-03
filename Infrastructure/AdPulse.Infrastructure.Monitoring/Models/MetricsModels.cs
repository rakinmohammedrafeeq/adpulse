using AdPulse.Core.Shared.Enums;

namespace AdPulse.Infrastructure.Monitoring.Models;

/// <summary>
/// ����ָ��
/// </summary>
public record StrategyMetrics
{
    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ִ��ID
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// ָ��ʱ���
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ִ�к�ʱ�����룩
    /// </summary>
    public double ExecutionTimeMs { get; init; }

    /// <summary>
    /// �ڴ�ʹ�������ֽڣ�
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// CPUʹ���ʣ��ٷֱȣ�
    /// </summary>
    public double CpuUsage { get; init; }

    /// <summary>
    /// ����ĺ�ѡ����
    /// </summary>
    public int ProcessedCount { get; init; }

    /// <summary>
    /// �ɹ����������
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// ʧ�ܴ��������
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// �������ʣ���/�룩
    /// </summary>
    public double ProcessingRate => ExecutionTimeMs > 0 ? ProcessedCount * 1000.0 / ExecutionTimeMs : 0;

    /// <summary>
    /// �ɹ���
    /// </summary>
    public double SuccessRate => ProcessedCount > 0 ? (double)SuccessCount / ProcessedCount : 0;

    /// <summary>
    /// �Զ���ָ��
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomMetrics { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// ����ָ��
/// </summary>
public record PerformanceMetrics
{
    /// <summary>
    /// �������
    /// </summary>
    public required string ComponentName { get; init; }

    /// <summary>
    /// ָ��ʱ���
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ��Ӧʱ�䣨���룩
    /// </summary>
    public double ResponseTimeMs { get; init; }

    /// <summary>
    /// ������������/�룩
    /// </summary>
    public double Throughput { get; init; }

    /// <summary>
    /// �����ʣ��ٷֱȣ�
    /// </summary>
    public double ErrorRate { get; init; }

    /// <summary>
    /// ������
    /// </summary>
    public int ConcurrentRequests { get; init; }

    /// <summary>
    /// ���г���
    /// </summary>
    public int QueueLength { get; init; }

    /// <summary>
    /// ��Դʹ�����
    /// </summary>
    public ResourceUsage ResourceUsage { get; init; } = new();

    /// <summary>
    /// �ӳٷֲ�
    /// </summary>
    public LatencyDistribution LatencyDistribution { get; init; } = new();

    /// <summary>
    /// ҵ��ָ��
    /// </summary>
    public IReadOnlyDictionary<string, double> BusinessMetrics { get; init; } =
        new Dictionary<string, double>();
}

/// <summary>
/// ҵ��ָ��
/// </summary>
public record BusinessMetrics
{
    /// <summary>
    /// ָ��ʱ���
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ���չʾ��
    /// </summary>
    public long Impressions { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public long Clicks { get; init; }

    /// <summary>
    /// ת����
    /// </summary>
    public long Conversions { get; init; }

    /// <summary>
    /// ���루�֣�
    /// </summary>
    public decimal Revenue { get; init; }

    /// <summary>
    /// �ɱ����֣�
    /// </summary>
    public decimal Cost { get; init; }

    /// <summary>
    /// �����
    /// </summary>
    public double ClickThroughRate => Impressions > 0 ? (double)Clicks / Impressions : 0;

    /// <summary>
    /// ת����
    /// </summary>
    public double ConversionRate => Clicks > 0 ? (double)Conversions / Clicks : 0;

    /// <summary>
    /// ƽ��ÿ�ε���ɱ����֣�
    /// </summary>
    public decimal CostPerClick => Clicks > 0 ? Cost / Clicks : 0;

    /// <summary>
    /// ƽ��ÿ��ת���ɱ����֣�
    /// </summary>
    public decimal CostPerConversion => Conversions > 0 ? Cost / Conversions : 0;

    /// <summary>
    /// Ͷ�ʻر���
    /// </summary>
    public decimal ReturnOnInvestment => Cost > 0 ? (Revenue - Cost) / Cost : 0;

    /// <summary>
    /// �����
    /// </summary>
    public double FillRate { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public double QualityScore { get; init; }

    /// <summary>
    /// ���������
    /// </summary>
    public int ActiveAdvertisers { get; init; }

    /// <summary>
    /// ��Ծ�������
    /// </summary>
    public int ActiveAds { get; init; }

    /// <summary>
    /// �Զ���ҵ��ָ��
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomMetrics { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// ִ��ͳ��
/// </summary>
public record ExecutionStatistics
{
    /// <summary>
    /// ִ��ID
    /// </summary>
    public required string ExecutionId { get; init; }

    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ��ʼʱ��
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime EndTime { get; init; }

    /// <summary>
    /// ִ��״̬
    /// </summary>
    public ExecutionStatus Status { get; init; } = ExecutionStatus.Running;

    /// <summary>
    /// �������ݴ�С
    /// </summary>
    public int InputSize { get; init; }

    /// <summary>
    /// ������ݴ�С
    /// </summary>
    public int OutputSize { get; init; }

    /// <summary>
    /// �����������
    /// </summary>
    public long ProcessedDataSize { get; init; }

    /// <summary>
    /// ���ݿ��������
    /// </summary>
    public int DatabaseOperations { get; init; }

    /// <summary>
    /// �����������
    /// </summary>
    public int CacheOperations { get; init; }

    /// <summary>
    /// ������ô���
    /// </summary>
    public int NetworkCalls { get; init; }

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// �����ٶȣ���/�룩
    /// </summary>
    public double ProcessingSpeed => Duration.TotalSeconds > 0 ? InputSize / Duration.TotalSeconds : 0;

    /// <summary>
    /// ��ϸͳ����Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> DetailedStats { get; init; } =
        new Dictionary<string, object>();
}

/// <summary>
/// ָ������
/// </summary>
public record MetricsBatch
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string BatchId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ����ʱ���
    /// </summary>
    public DateTime BatchTimestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ����ָ���б�
    /// </summary>
    public IReadOnlyList<StrategyMetrics> StrategyMetrics { get; init; } = Array.Empty<StrategyMetrics>();

    /// <summary>
    /// ����ָ���б�
    /// </summary>
    public IReadOnlyList<PerformanceMetrics> PerformanceMetrics { get; init; } = Array.Empty<PerformanceMetrics>();

    /// <summary>
    /// ҵ��ָ���б�
    /// </summary>
    public IReadOnlyList<BusinessMetrics> BusinessMetrics { get; init; } = Array.Empty<BusinessMetrics>();

    /// <summary>
    /// ���δ�С
    /// </summary>
    public int BatchSize => StrategyMetrics.Count + PerformanceMetrics.Count + BusinessMetrics.Count;
}

/// <summary>
/// �澯�¼�
/// </summary>
public record AlertEvent
{
    /// <summary>
    /// �澯ID
    /// </summary>
    public string AlertId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// �澯ʱ��
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// �澯����
    /// </summary>
    public AlertLevel Level { get; init; } = AlertLevel.Warning;

    /// <summary>
    /// �澯����
    /// </summary>
    public required string AlertType { get; init; }

    /// <summary>
    /// �澯��Ϣ
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// �澯��Դ
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// ��Ӱ������
    /// </summary>
    public IReadOnlyList<string> AffectedComponents { get; init; } = Array.Empty<string>();

    /// <summary>
    /// �澯����
    /// </summary>
    public IReadOnlyDictionary<string, object> Details { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// �Ƽ�����
    /// </summary>
    public IReadOnlyList<string> RecommendedActions { get; init; } = Array.Empty<string>();

    /// <summary>
    /// �Ƿ��ѽ��
    /// </summary>
    public bool IsResolved { get; init; } = false;

    /// <summary>
    /// ���ʱ��
    /// </summary>
    public DateTime? ResolvedAt { get; init; }
}

/// <summary>
/// �����־
/// </summary>
public record AuditLog
{
    /// <summary>
    /// ��־ID
    /// </summary>
    public string LogId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ʱ���
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ��������
    /// </summary>
    public required string Action { get; init; }

    /// <summary>
    /// �����û�
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// ��Դ����
    /// </summary>
    public required string ResourceType { get; init; }

    /// <summary>
    /// ��ԴID
    /// </summary>
    public required string ResourceId { get; init; }

    /// <summary>
    /// �������
    /// </summary>
    public AuditResult Result { get; init; } = AuditResult.Success;

    /// <summary>
    /// ��������
    /// </summary>
    public IReadOnlyDictionary<string, object> Details { get; init; } =
        new Dictionary<string, object>();

    /// <summary>
    /// IP��ַ
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// �û�����
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// �ỰID
    /// </summary>
    public string? SessionId { get; init; }
}

/// <summary>
/// ��Դʹ�����
/// </summary>
public record ResourceUsage
{
    /// <summary>
    /// CPUʹ���ʣ��ٷֱȣ�
    /// </summary>
    public double CpuUsage { get; init; }

    /// <summary>
    /// �ڴ�ʹ�������ֽڣ�
    /// </summary>
    public long MemoryUsage { get; init; }

    /// <summary>
    /// ����ʹ�������ֽڣ�
    /// </summary>
    public long DiskUsage { get; init; }

    /// <summary>
    /// �������������ֽ�/�룩
    /// </summary>
    public long NetworkInBytes { get; init; }

    /// <summary>
    /// �������������ֽ�/�룩
    /// </summary>
    public long NetworkOutBytes { get; init; }

    /// <summary>
    /// ������
    /// </summary>
    public int ConnectionCount { get; init; }

    /// <summary>
    /// �߳���
    /// </summary>
    public int ThreadCount { get; init; }
}

/// <summary>
/// �ӳٷֲ�
/// </summary>
public record LatencyDistribution
{
    /// <summary>
    /// P50�ӳ٣����룩
    /// </summary>
    public double P50 { get; init; }

    /// <summary>
    /// P90�ӳ٣����룩
    /// </summary>
    public double P90 { get; init; }

    /// <summary>
    /// P95�ӳ٣����룩
    /// </summary>
    public double P95 { get; init; }

    /// <summary>
    /// P99�ӳ٣����룩
    /// </summary>
    public double P99 { get; init; }

    /// <summary>
    /// ��С�ӳ٣����룩
    /// </summary>
    public double Min { get; init; }

    /// <summary>
    /// ����ӳ٣����룩
    /// </summary>
    public double Max { get; init; }

    /// <summary>
    /// ƽ���ӳ٣����룩
    /// </summary>
    public double Average { get; init; }

    /// <summary>
    /// ��׼��
    /// </summary>
    public double StandardDeviation { get; init; }
}