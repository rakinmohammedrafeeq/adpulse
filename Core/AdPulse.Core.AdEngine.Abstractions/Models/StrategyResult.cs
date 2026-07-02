using AdPulse.Core.Domain.Entities;
using AdPulse.Core.Domain.ValueObjects;

namespace AdPulse.Core.AdEngine.Abstractions.Models;

/// <summary>
/// ����ִ�н��
/// </summary>
public record StrategyResult
{
    /// <summary>
    /// ִ���Ƿ�ɹ�
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// �����Ĺ���ѡ�б�
    /// </summary>
    public required IReadOnlyList<AdCandidate> ProcessedCandidates { get; init; }

    /// <summary>
    /// ����ִ��ͳ����Ϣ
    /// </summary>
    public required StrategyExecutionStatistics Statistics { get; init; }

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<StrategyError> Errors { get; init; } = Array.Empty<StrategyError>();

    /// <summary>
    /// Ԫ������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// ִ��ID
    /// </summary>
    public string ExecutionId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ����ID
    /// </summary>
    public required string StrategyId { get; init; }

    /// <summary>
    /// ִ�п�ʼʱ��
    /// </summary>
    public DateTime StartTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ִ�н���ʱ��
    /// </summary>
    public DateTime EndTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ִ��ʱ��
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// �����ɹ����
    /// </summary>
    public static StrategyResult Success(
        string strategyId,
        IReadOnlyList<AdCandidate> candidates,
        StrategyExecutionStatistics statistics,
        IReadOnlyDictionary<string, object>? metadata = null)
    {
        return new StrategyResult
        {
            IsSuccess = true,
            StrategyId = strategyId,
            ProcessedCandidates = candidates,
            Statistics = statistics,
            Metadata = metadata ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// ����ʧ�ܽ��
    /// </summary>
    public static StrategyResult Failure(
        string strategyId,
        IReadOnlyList<StrategyError> errors,
        StrategyExecutionStatistics statistics,
        IReadOnlyList<AdCandidate>? candidates = null)
    {
        return new StrategyResult
        {
            IsSuccess = false,
            StrategyId = strategyId,
            ProcessedCandidates = candidates ?? Array.Empty<AdCandidate>(),
            Statistics = statistics,
            Errors = errors
        };
    }
}



