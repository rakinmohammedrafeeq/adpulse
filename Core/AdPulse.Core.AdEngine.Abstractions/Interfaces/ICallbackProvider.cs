namespace AdPulse.Core.AdEngine.Abstractions.Interfaces;

/// <summary>
/// �ص��ṩ�߽ӿڣ������ṩ���Ͱ�ȫ�Ļص����ʻ���
/// </summary>
public interface ICallbackProvider
{
    /// <summary>
    /// �������ͻ�ȡ�ص��ӿڣ����Ͱ�ȫ��
    /// </summary>
    /// <typeparam name="T">�ص��ӿ�����</typeparam>
    /// <returns>ָ�����͵Ļص��ӿ�</returns>
    /// <exception cref="CallbackNotFoundException">���ص�������ʱ�׳�</exception>
    T GetCallback<T>() where T : class, IAdEngineCallback;

    /// <summary>
    /// �������ƺ����ͻ�ȡ�ص��ӿ�
    /// </summary>
    /// <typeparam name="T">�ص��ӿ�����</typeparam>
    /// <param name="name">�ص�����</param>
    /// <returns>ָ�����͵Ļص��ӿ�</returns>
    /// <exception cref="CallbackNotFoundException">���ص�������ʱ�׳�</exception>
    T GetCallback<T>(string name) where T : class, IAdEngineCallback;

    /// <summary>
    /// ����Ƿ����ָ�����͵Ļص�
    /// </summary>
    /// <typeparam name="T">�ص��ӿ�����</typeparam>
    /// <returns>��������򷵻�true�����򷵻�false</returns>
    bool HasCallback<T>() where T : class, IAdEngineCallback;

    /// <summary>
    /// ����Ƿ����ָ�����ƵĻص�
    /// </summary>
    /// <param name="callbackName">�ص�����</param>
    /// <returns>��������򷵻�true�����򷵻�false</returns>
    bool HasCallback(string callbackName);

    /// <summary>
    /// ��ȡ���п��õĻص��ӿ�
    /// </summary>
    /// <returns>�ص����ƺͽӿڵ�ֻ���ֵ�</returns>
    IReadOnlyDictionary<string, IAdEngineCallback> GetAllCallbacks();

    /// <summary>
    /// ���Ի�ȡ�ص��ӿڣ������쳣��
    /// </summary>
    /// <typeparam name="T">�ص��ӿ�����</typeparam>
    /// <param name="callback">����Ļص��ӿ�</param>
    /// <returns>�����ȡ�ɹ��򷵻�true�����򷵻�false</returns>
    bool TryGetCallback<T>(out T? callback) where T : class, IAdEngineCallback;

    /// <summary>
    /// ���Ը������ƻ�ȡ�ص��ӿڣ������쳣��
    /// </summary>
    /// <typeparam name="T">�ص��ӿ�����</typeparam>
    /// <param name="name">�ص�����</param>
    /// <param name="callback">����Ļص��ӿ�</param>
    /// <returns>�����ȡ�ɹ��򷵻�true�����򷵻�false</returns>
    bool TryGetCallback<T>(string name, out T? callback) where T : class, IAdEngineCallback;
}