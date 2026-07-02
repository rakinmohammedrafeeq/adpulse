namespace AdPulse.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ��������
/// </summary>
public record StrategyConfig
{
    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ���ð汾
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// ִ�����ȼ�
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// ��ʱ���ã����룩
    /// </summary>
    public int TimeoutMs { get; init; } = 10000;

    /// <summary>
    /// ���Դ���
    /// </summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>
    /// ���ò���
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// �����ض�����
    /// </summary>
    public IReadOnlyDictionary<string, object> EnvironmentConfig { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Ȩ������
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Weights { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// ��ֵ����
    /// </summary>
    public IReadOnlyDictionary<string, decimal> Thresholds { get; init; } = new Dictionary<string, decimal>();

    /// <summary>
    /// ���Կ���
    /// </summary>
    public IReadOnlyDictionary<string, bool> FeatureFlags { get; init; } = new Dictionary<string, bool>();

    /// <summary>
    /// A/B��������
    /// </summary>
    public ABTestConfig? ABTestConfig { get; init; }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ���ô�����
    /// </summary>
    public string? CreatedBy { get; init; }

    /// <summary>
    /// ���ø�����
    /// </summary>
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// ��ȡ����ֵ
    /// </summary>
    public T GetParameter<T>(string key, T defaultValue = default!)
    {
        if (Parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// ��ȡȨ��ֵ
    /// </summary>
    public decimal GetWeight(string key, decimal defaultValue = 1.0m)
    {
        return Weights.TryGetValue(key, out var weight) ? weight : defaultValue;
    }

    /// <summary>
    /// ��ȡ��ֵ
    /// </summary>
    public decimal GetThreshold(string key, decimal defaultValue = 0.5m)
    {
        return Thresholds.TryGetValue(key, out var threshold) ? threshold : defaultValue;
    }

    /// <summary>
    /// ��ȡ���Կ���״̬
    /// </summary>
    public bool GetFeatureFlag(string key, bool defaultValue = false)
    {
        return FeatureFlags.TryGetValue(key, out var flag) ? flag : defaultValue;
    }
}




