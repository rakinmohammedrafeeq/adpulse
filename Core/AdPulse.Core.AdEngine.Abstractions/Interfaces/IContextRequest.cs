using AdPulse.Core.AdEngine.Abstractions.Enums;
using AdPulse.Core.AdEngine.Abstractions.Models;
using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Shared.Entities;

namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// ����������ӿڣ����ڷ�װ��ȡ�ض���������������Ĳ���
/// </summary>
/// <typeparam name="T">��������������ͣ�����ʵ��ITargetingContext�ӿ�</typeparam>
public interface IContextRequest<T> where T : class, ITargetingContext
{
    /// <summary>
    /// ���������ͱ�ʶ
    /// </summary>
    string ContextType { get; }

    /// <summary>
    /// ��������ֵ�
    /// </summary>
    IReadOnlyDictionary<string, object> Parameters { get; }

    /// <summary>
    /// ����ʱʱ��
    /// </summary>
    TimeSpan Timeout { get; }

    /// <summary>
    /// �����������
    /// </summary>
    CachePolicy CachePolicy { get; }

    /// <summary>
    /// ����Ψһ��ʶ��
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// �������ȼ�
    /// </summary>
    RequestPriority Priority { get; }

    /// <summary>
    /// �Ƿ�����ʹ�û�������
    /// </summary>
    bool AllowCached { get; }

    /// <summary>
    /// ���ɽ��ܵ������ӳ�ʱ��
    /// </summary>
    TimeSpan MaxDataAge { get; }

    /// <summary>
    /// ��ȡǿ���͵Ĳ���ֵ
    /// </summary>
    /// <typeparam name="TValue">����ֵ����</typeparam>
    /// <param name="key">������</param>
    /// <returns>����ֵ</returns>
    TValue? GetParameter<TValue>(string key);

    /// <summary>
    /// ����Ƿ����ָ���Ĳ���
    /// </summary>
    /// <param name="key">������</param>
    /// <returns>�����������true�����򷵻�false</returns>
    bool HasParameter(string key);

    /// <summary>
    /// ��֤�����������Ч��
    /// </summary>
    /// <returns>��֤���</returns>
    ValidationResult Validate();
}