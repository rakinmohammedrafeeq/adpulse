using AdPulse.Core.AdEngine.Abstractions.Models;
using AdPulse.Core.Domain.BusinessRules;
using AdPulse.Core.Shared.Entities;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ����������ù���ص��ӿ�
/// </summary>
public interface IAdEngineConfigCallback : IAdEngineCallback
{
    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="strategyId">����ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������</returns>
    Task<StrategyConfig?> GetStrategyConfigAsync(
        string strategyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡҵ���������
    /// </summary>
    /// <param name="ruleType">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ҵ�����</returns>
    Task<BusinessRules> GetBusinessRulesAsync(
        string ruleType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��ֵ����
    /// </summary>
    /// <param name="paramType">��������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��ֵ�����ֵ�</returns>
    Task<IReadOnlyDictionary<string, object>> GetThresholdParametersAsync(
        string paramType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡA/B��������
    /// </summary>
    /// <param name="experimentId">ʵ��ID</param>
    /// <param name="userId">�û�ID</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>A/B��������</returns>
    Task<ABTestConfig?> GetABTestConfigAsync(
        string experimentId,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��������
    /// </summary>
    /// <param name="featureKey">������</param>
    /// <param name="defaultValue">Ĭ��ֵ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>����ֵ</returns>
    Task<T> GetFeatureFlagAsync<T>(
        string featureKey,
        T defaultValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ������ȡ��������
    /// </summary>
    /// <param name="strategyIds">����ID�б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���������ֵ�</returns>
    Task<IReadOnlyDictionary<string, StrategyConfig>> GetStrategyConfigsBatchAsync(
        IReadOnlyList<string> strategyIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ��ȡ��̬����
    /// </summary>
    /// <param name="section">���ý�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��̬��������</returns>
    Task<DynamicParameters> GetDynamicParametersAsync(
        string section,
        CancellationToken cancellationToken = default);
}