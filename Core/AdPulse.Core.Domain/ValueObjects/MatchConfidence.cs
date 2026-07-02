using AdPulse.Core.Domain.Common;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ƥ�����Ŷ�ֵ����
/// ��ʾOverallMatchResult��������ͳ�����Ŷȣ��ṩƥ�����ɿ��Ե���������
/// </summary>
public class MatchConfidence : ValueObject
{
    /// <summary>
    /// ���Ŷȷ�����0-1��
    /// </summary>
    public decimal ConfidenceScore { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public int SampleSize { get; private set; }

    /// <summary>
    /// ��׼��
    /// </summary>
    public decimal StandardDeviation { get; private set; }

    /// <summary>
    /// ��������
    /// </summary>
    public decimal ConfidenceInterval { get; private set; }

    /// <summary>
    /// ���Ŷȵȼ�ö��
    /// </summary>
    public ConfidenceLevel Level { get; private set; }

    /// <summary>
    /// ���Ŷȼ��㷽������
    /// </summary>
    public string CalculationMethod { get; private set; }

    /// <summary>
    /// ��ϸͳ��ָ��
    /// </summary>
    public IReadOnlyList<ContextProperty> StatisticalMetrics { get; private set; }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// �Ƿ�ɿ���ʶ
    /// </summary>
    public bool IsReliable { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private MatchConfidence(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        ConfidenceLevel level,
        string calculationMethod,
        IReadOnlyList<ContextProperty> statisticalMetrics,
        DateTime lastUpdated,
        bool isReliable)
    {
        ConfidenceScore = confidenceScore;
        SampleSize = sampleSize;
        StandardDeviation = standardDeviation;
        ConfidenceInterval = confidenceInterval;
        Level = level;
        CalculationMethod = calculationMethod;
        StatisticalMetrics = statisticalMetrics;
        LastUpdated = lastUpdated;
        IsReliable = isReliable;
    }

    /// <summary>
    /// ����ƥ�����Ŷ�
    /// </summary>
    public static MatchConfidence Create(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        string calculationMethod = "Standard",
        IEnumerable<ContextProperty>? statisticalMetrics = null)
    {
        ValidateInputs(confidenceScore, sampleSize, standardDeviation, confidenceInterval);

        var level = DetermineConfidenceLevel(confidenceScore);
        var isReliable = DetermineReliability(confidenceScore, sampleSize);
        var metrics = statisticalMetrics?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchConfidence(
            confidenceScore,
            sampleSize,
            standardDeviation,
            confidenceInterval,
            level,
            calculationMethod,
            metrics,
            DateTime.UtcNow,
            isReliable);
    }

    /// <summary>
    /// ����ƥ�����Ŷȣ����ֵ������
    /// </summary>
    public static MatchConfidence CreateFromDictionary(
        decimal confidenceScore,
        int sampleSize,
        decimal standardDeviation,
        decimal confidenceInterval,
        string calculationMethod = "Standard",
        IDictionary<string, decimal>? statisticalMetrics = null)
    {
        var metrics = statisticalMetrics?.Select(kvp => new ContextProperty(
            kvp.Key,
            kvp.Value.ToString(),
            "Decimal",
            "StatisticalMetric",
            false,
            1.0m,
            null,
            "MatchConfidence")) ?? Enumerable.Empty<ContextProperty>();

        return Create(confidenceScore, sampleSize, standardDeviation, confidenceInterval, calculationMethod, metrics);
    }

    /// <summary>
    /// ����Ĭ�����Ŷȣ�����ȱ��ͳ�����ݵĳ�����
    /// </summary>
    public static MatchConfidence CreateDefault()
    {
        return Create(
            0.5m,
            1,
            0.0m,
            0.0m,
            "Default"
        );
    }

    /// <summary>
    /// ���������Ŷ�
    /// </summary>
    public static MatchConfidence CreateHigh(int sampleSize, decimal standardDeviation = 0.1m)
    {
        return Create(
            0.85m,
            sampleSize,
            standardDeviation,
            0.05m,
            "HighConfidence"
        );
    }

    /// <summary>
    /// ��ȡͳ�ƶ���ֵ
    /// </summary>
    public T? GetStatisticalMetric<T>(string metricKey) where T : struct
    {
        var metric = StatisticalMetrics.FirstOrDefault(m => m.PropertyKey == metricKey);
        return metric?.GetValue<T>();
    }

    /// <summary>
    /// ��ȡͳ�ƶ���ֵ���������ͣ�
    /// </summary>
    public T? GetStatisticalMetricRef<T>(string metricKey) where T : class
    {
        var metric = StatisticalMetrics.FirstOrDefault(m => m.PropertyKey == metricKey);
        return metric?.GetValue<T>();
    }

    /// <summary>
    /// ��ȡ����ͳ�ƶ�����Ϊ�ֵ䣨�����ݣ�
    /// </summary>
    public Dictionary<string, decimal> GetStatisticalMetricsAsDictionary()
    {
        var result = new Dictionary<string, decimal>();
        foreach (var metric in StatisticalMetrics)
        {
            if (decimal.TryParse(metric.PropertyValue, out decimal value))
            {
                result[metric.PropertyKey] = value;
            }
        }
        return result;
    }

    /// <summary>
    /// ����Ƿ�����ض���ͳ�ƶ���
    /// </summary>
    public bool HasStatisticalMetric(string metricKey)
    {
        return StatisticalMetrics.Any(m => m.PropertyKey == metricKey);
    }

    /// <summary>
    /// ��ȡ���Ŷȵȼ�
    /// </summary>
    public ConfidenceLevel GetConfidenceLevel()
    {
        return Level;
    }

    /// <summary>
    /// ��������������ͳ����Ϣ
    /// </summary>
    public MatchConfidence UpdateStatistics(IList<decimal> newSamples)
    {
        if (newSamples == null || !newSamples.Any())
            return this;

        var totalSamples = SampleSize + newSamples.Count;
        var allSamples = new List<decimal> { ConfidenceScore }.Concat(newSamples).ToList();

        var newMean = allSamples.Average();
        var newStandardDeviation = CalculateStandardDeviation(allSamples, newMean);
        var newConfidenceInterval = CalculateConfidenceInterval(newStandardDeviation, totalSamples);

        // �������µ�ͳ�ƶ���������µ�����ͳ����Ϣ
        var updatedMetrics = StatisticalMetrics.ToList();
        updatedMetrics.Add(new ContextProperty("SampleCount", newSamples.Count.ToString(), "Int32", "UpdatedStatistic", false, 1.0m, null, "UpdateStatistics"));
        updatedMetrics.Add(new ContextProperty("NewMean", newMean.ToString(), "Decimal", "UpdatedStatistic", false, 1.0m, null, "UpdateStatistics"));

        return Create(
            newMean,
            totalSamples,
            newStandardDeviation,
            newConfidenceInterval,
            $"{CalculationMethod}_Updated",
            updatedMetrics
        );
    }

    /// <summary>
    /// ��ȡͳ��ժҪ
    /// </summary>
    public Dictionary<string, object> GetStatisticalSummary()
    {
        return new Dictionary<string, object>
        {
            ["ConfidenceScore"] = ConfidenceScore,
            ["Level"] = Level.ToString(),
            ["SampleSize"] = SampleSize,
            ["StandardDeviation"] = StandardDeviation,
            ["ConfidenceInterval"] = ConfidenceInterval,
            ["IsReliable"] = IsReliable,
            ["ReliabilityRating"] = GetReliabilityRating().ToString(),
            ["CalculationMethod"] = CalculationMethod,
            ["LastUpdated"] = LastUpdated
        };
    }

    /// <summary>
    /// �Ƿ񳬹���ֵ
    /// </summary>
    public bool IsAboveThreshold(decimal threshold)
    {
        return ConfidenceScore >= threshold;
    }

    /// <summary>
    /// ��ȡ�ɿ�������
    /// </summary>
    public ReliabilityRating GetReliabilityRating()
    {
        if (!IsReliable) return ReliabilityRating.Unreliable;

        return Level switch
        {
            ConfidenceLevel.VeryHigh => ReliabilityRating.FullReliability,
            ConfidenceLevel.High => ReliabilityRating.HighReliability,
            ConfidenceLevel.Medium => ReliabilityRating.BasicReliability,
            ConfidenceLevel.Low => ReliabilityRating.LimitedReliability,
            _ => ReliabilityRating.Unreliable
        };
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ConfidenceScore;
        yield return SampleSize;
        yield return StandardDeviation;
        yield return ConfidenceInterval;
        yield return Level;
        yield return CalculationMethod;
        yield return IsReliable;

        // ���ϵ�����ԱȽ�
        foreach (var metric in StatisticalMetrics.OrderBy(m => m.PropertyKey))
        {
            yield return metric;
        }
    }

    /// <summary>
    /// ȷ�����Ŷȵȼ�
    /// </summary>
    private static ConfidenceLevel DetermineConfidenceLevel(decimal confidenceScore)
    {
        return confidenceScore switch
        {
            >= 0.8m => ConfidenceLevel.VeryHigh,
            >= 0.6m => ConfidenceLevel.High,
            >= 0.4m => ConfidenceLevel.Medium,
            >= 0.2m => ConfidenceLevel.Low,
            _ => ConfidenceLevel.VeryLow
        };
    }

    /// <summary>
    /// ȷ���ɿ���
    /// </summary>
    private static bool DetermineReliability(decimal confidenceScore, int sampleSize)
    {
        // ���Ŷȳ���0.6��������������10����Ϊ�ǿɿ���
        return confidenceScore >= 0.6m && sampleSize >= 10;
    }

    /// <summary>
    /// �����׼��
    /// </summary>
    private static decimal CalculateStandardDeviation(IList<decimal> samples, decimal mean)
    {
        if (samples.Count <= 1) return 0m;

        var variance = samples.Select(x => (x - mean) * (x - mean)).Average();
        return (decimal)Math.Sqrt((double)variance);
    }

    /// <summary>
    /// ������������
    /// </summary>
    private static decimal CalculateConfidenceInterval(decimal standardDeviation, int sampleSize)
    {
        if (sampleSize <= 1) return 0m;

        // ʹ��95%���������tֵ���Ƽ���
        var tValue = 1.96m; // 95%���������zֵ
        return tValue * standardDeviation / (decimal)Math.Sqrt(sampleSize);
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(decimal confidenceScore, int sampleSize, decimal standardDeviation, decimal confidenceInterval)
    {
        if (confidenceScore < 0m || confidenceScore > 1m)
            throw new ArgumentException("���Ŷȷ���������0-1֮��", nameof(confidenceScore));

        if (sampleSize < 1)
            throw new ArgumentException("���������������0", nameof(sampleSize));

        if (standardDeviation < 0m)
            throw new ArgumentException("��׼���Ϊ����", nameof(standardDeviation));

        if (confidenceInterval < 0m)
            throw new ArgumentException("�������䲻��Ϊ����", nameof(confidenceInterval));
    }
}