using AdPulse.Core.Domain.Targeting;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ������������Ļص��ӿ� - ֧�ַ��ͻ��������Ļ�ȡ����
/// </summary>
public interface IAdEngineContextCallback : IAdEngineCallback
{
    /// <summary>
    /// ���ͷ�ʽ��ȡ��������Ϣ
    /// </summary>
    /// <typeparam name="T">���������ͣ�����ʵ��ITargetingContext�ӿ�</typeparam>
    /// <param name="request">�������������</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ�����͵���������Ϣ</returns>
    Task<T> GetContextAsync<T>(IContextRequest<T> request, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// �ֵ������ʽ��ȡ��������Ϣ
    /// </summary>
    /// <typeparam name="T">���������ͣ�����ʵ��ITargetingContext�ӿ�</typeparam>
    /// <param name="contextType">���������ͱ�ʶ</param>
    /// <param name="parameters">��������ֵ�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>ָ�����͵���������Ϣ</returns>
    Task<T> GetContextAsync<T>(string contextType, IReadOnlyDictionary<string, object> parameters, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// ������ȡ��������Ϣ
    /// </summary>
    /// <typeparam name="T">���������ͣ�����ʵ��ITargetingContext�ӿ�</typeparam>
    /// <param name="requests">�����������б�</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>��������Ϣ�б�</returns>
    Task<IReadOnlyList<T>> GetBatchContextAsync<T>(IReadOnlyList<IContextRequest<T>> requests, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;

    /// <summary>
    /// ����Ƿ�֧��ָ�������������ͣ����ͷ�ʽ��
    /// </summary>
    /// <typeparam name="T">����������</typeparam>
    /// <returns>���֧�ַ���true�����򷵻�false</returns>
    bool IsContextTypeSupported<T>() where T : class, ITargetingContext;

    /// <summary>
    /// ����Ƿ�֧��ָ�������������ͣ��ַ�����ʽ��
    /// </summary>
    /// <param name="contextType">���������ͱ�ʶ</param>
    /// <returns>���֧�ַ���true�����򷵻�false</returns>
    bool IsContextTypeSupported(string contextType);

    /// <summary>
    /// ��ȡ����֧�ֵ������������б�
    /// </summary>
    /// <returns>֧�ֵ����������ͱ�ʶ�б�</returns>
    IReadOnlyList<string> GetSupportedContextTypes();

    /// <summary>
    /// ���Ի�ȡ��������Ϣ�����׳��쳣
    /// </summary>
    /// <typeparam name="T">����������</typeparam>
    /// <param name="request">�������������</param>
    /// <param name="context">�������������Ϣ</param>
    /// <param name="cancellationToken">ȡ������</param>
    /// <returns>�����ȡ�ɹ�����true�����򷵻�false</returns>
    Task<(bool Success, T? Context)> TryGetContextAsync<T>(IContextRequest<T> request, CancellationToken cancellationToken = default)
        where T : class, ITargetingContext;
}