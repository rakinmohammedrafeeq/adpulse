namespace AdPulse.Core.AdEngine.Abstractions.Constants;

/// <summary>
/// ���Գ���
/// </summary>
public static class StrategyConstants
{
    /// <summary>
    /// Ĭ�ϲ������ȼ�
    /// </summary>
    public const int DefaultPriority = 5;

    /// <summary>
    /// ������ȼ�
    /// </summary>
    public const int HighestPriority = 1;

    /// <summary>
    /// ������ȼ�
    /// </summary>
    public const int LowestPriority = 10;

    /// <summary>
    /// Ĭ�ϳ�ʱʱ�䣨���룩
    /// </summary>
    public const int DefaultTimeoutMs = 10000;

    /// <summary>
    /// ��С��ʱʱ�䣨���룩
    /// </summary>
    public const int MinTimeoutMs = 100;

    /// <summary>
    /// ���ʱʱ�䣨���룩
    /// </summary>
    public const int MaxTimeoutMs = 60000;

    /// <summary>
    /// Ĭ�����Դ���
    /// </summary>
    public const int DefaultMaxRetries = 3;

    /// <summary>
    /// ������Դ���
    /// </summary>
    public const int MaxRetries = 10;

    /// <summary>
    /// Ĭ���������С
    /// </summary>
    public const int DefaultBatchSize = 100;

    /// <summary>
    /// ����������С
    /// </summary>
    public const int MaxBatchSize = 10000;

    /// <summary>
    /// ���԰汾��ʽ
    /// </summary>
    public const string VersionFormat = @"^\d+\.\d+\.\d+$";

    /// <summary>
    /// ����ID��ʽ
    /// </summary>
    public const string StrategyIdFormat = @"^[a-zA-Z][a-zA-Z0-9_]*$";
}









