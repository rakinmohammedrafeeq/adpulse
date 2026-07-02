using AdPulse.Core.AdEngine.Abstractions.Enums;
using AdPulse.Core.AdEngine.Abstractions.Models;
using AdPulse.Core.Domain.Entities;
using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Shared.Entities;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ��洦�����ͳһ�ӿ�
/// </summary>
public interface IAdProcessingStrategy
{
    /// <summary>
    /// ����Ψһ��ʶ��
    /// </summary>
    string StrategyId { get; }

    /// <summary>
    /// ���԰汾��Ϣ
    /// </summary>
    string Version { get; }

    /// <summary>
    /// ��������
    /// </summary>
    StrategyType Type { get; }

    /// <summary>
    /// ִ�����ȼ�����ֵԽС���ȼ�Խ�ߣ�
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// �����Ƿ�����
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Ԥ��ִ��ʱ��
    /// </summary>
    TimeSpan ExpectedExecutionTime { get; }

    /// <summary>
    /// �Ƿ�֧�ֲ���ִ��
    /// </summary>
    bool CanRunInParallel { get; }

    /// <summary>
    /// ִ�в���
    /// </summary>
    /// <param name="context">��洦��������</param>
    /// <param name="candidates">����ѡ�б�</param>
    /// <param name="callbackProvider">�ص��ṩ��</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ִ�н��</returns>
    Task<StrategyResult> ExecuteAsync(
        AdContext context,
        IReadOnlyList<AdCandidate> candidates,
        ICallbackProvider callbackProvider,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��֤��������
    /// </summary>
    /// <param name="config">��������</param>
    /// <returns>��֤���</returns>
    ValidationResult ValidateConfiguration(StrategyConfig config);

    /// <summary>
    /// ��ȡ����Ԫ����
    /// </summary>
    /// <returns>����Ԫ����</returns>
    StrategyMetadata GetMetadata();
}