namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// �������ص��ӿڻ���
/// </summary>
public interface IAdEngineCallback
{
    /// <summary>
    /// �ص����ͱ�ʶ
    /// </summary>
    string CallbackType { get; }

    /// <summary>
    /// �ص�����
    /// </summary>
    string CallbackName { get; }

    /// <summary>
    /// �Ƿ����
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>���������</returns>
    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}