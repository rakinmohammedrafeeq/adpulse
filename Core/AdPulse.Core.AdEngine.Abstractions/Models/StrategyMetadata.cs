using AdPulse.Core.AdEngine.Abstractions.Enums;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ����Ԫ����
/// </summary>
public record StrategyMetadata
{
    /// <summary>
    /// ����Ψһ��ʶ��
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// ���԰汾
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public required StrategyType Type { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// ��������
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// ִ�����ȼ�
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// �Ƿ�֧�ֲ���ִ��
    /// </summary>
    public bool CanRunInParallel { get; init; } = true;

    /// <summary>
    /// Ԥ��ִ��ʱ��
    /// </summary>
    public TimeSpan EstimatedExecutionTime { get; init; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// ��Դ����
    /// </summary>
    public ResourceRequirement ResourceRequirement { get; init; } = new();

    /// <summary>
    /// ����ص��ӿ�����
    /// </summary>
    public IReadOnlyList<Type> RequiredCallbackTypes { get; init; } = Array.Empty<Type>();

    /// <summary>
    /// ����ص�����
    /// </summary>
    public IReadOnlyList<string> RequiredCallbackNames { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ����ģʽ����
    /// </summary>
    public IReadOnlyDictionary<string, object> ConfigurationSchema { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ֧�ֵĹ�������
    /// </summary>
    public IReadOnlyList<string> SupportedFeatures { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ���Ա�ǩ
    /// </summary>
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// ���ݵĹ������
    /// </summary>
    public IReadOnlyList<AdType> CompatibleAdTypes { get; init; } = Array.Empty<AdType>();

    /// <summary>
    /// ��С֧�ֵ�������
    /// </summary>
    public int MinDataSize { get; init; } = 1;

    /// <summary>
    /// ���֧�ֵ�������
    /// </summary>
    public int MaxDataSize { get; init; } = int.MaxValue;

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ��չ����
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedProperties { get; init; } = new Dictionary<string, object>();
}

