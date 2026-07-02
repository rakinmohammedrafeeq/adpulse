namespace AdPulse.Core.AdEngine.Abstractions.Models;

/// <summary>
/// �����������
/// </summary>
public sealed class CachePolicy
{
    /// <summary>
    /// �����������
    /// </summary>
    public Enums.CachePolicyType Type { get; init; } = Enums.CachePolicyType.CacheFirst;

    /// <summary>
    /// ������Ч��
    /// </summary>
    public TimeSpan? TTL { get; init; }

    /// <summary>
    /// �Ƿ�������ڻ���
    /// </summary>
    public bool AllowStale { get; init; } = false;

    /// <summary>
    /// ���ڻ���������������
    /// </summary>
    public TimeSpan? MaxStaleAge { get; init; }

    /// <summary>
    /// �����ǰ׺
    /// </summary>
    public string? KeyPrefix { get; init; }

    /// <summary>
    /// �Ƿ�ѹ����������
    /// </summary>
    public bool CompressData { get; init; } = false;

    /// <summary>
    /// �������ȼ�
    /// </summary>
    public int Priority { get; init; } = 1;

    /// <summary>
    /// �Զ����ǩ
    /// </summary>
    public IReadOnlyDictionary<string, string>? Tags { get; init; }

    /// <summary>
    /// Ĭ�ϻ������
    /// </summary>
    public static readonly CachePolicy Default = new()
    {
        Type = Enums.CachePolicyType.CacheFirst,
        TTL = TimeSpan.FromMinutes(5),
        AllowStale = true,
        MaxStaleAge = TimeSpan.FromMinutes(10)
    };

    /// <summary>
    /// �޻������
    /// </summary>
    public static readonly CachePolicy NoCache = new()
    {
        Type = Enums.CachePolicyType.NoCache
    };

    /// <summary>
    /// ���������
    /// </summary>
    public static readonly CachePolicy CacheOnly = new()
    {
        Type = Enums.CachePolicyType.CacheOnly,
        AllowStale = true
    };

    /// <summary>
    /// �����Զ��建�����
    /// </summary>
    public static CachePolicy Create(
        Enums.CachePolicyType type,
        TimeSpan? ttl = null,
        bool allowStale = false,
        TimeSpan? maxStaleAge = null)
    {
        return new CachePolicy
        {
            Type = type,
            TTL = ttl,
            AllowStale = allowStale,
            MaxStaleAge = maxStaleAge
        };
    }
}