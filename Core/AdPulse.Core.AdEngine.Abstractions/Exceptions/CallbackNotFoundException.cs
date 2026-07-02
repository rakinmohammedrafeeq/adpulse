namespace AdPulse.Core.AdEngine.Abstractions.Exceptions;

/// <summary>
/// �ص�δ�ҵ��쳣
/// </summary>
public class CallbackNotFoundException : Exception
{
    /// <summary>
    /// �ص�����
    /// </summary>
    public Type? CallbackType { get; }

    /// <summary>
    /// �ص�����
    /// </summary>
    public string? CallbackName { get; }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(Type callbackType)
        : base($"Callback of type '{callbackType.Name}' not found")
    {
        CallbackType = callbackType;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public CallbackNotFoundException(string callbackName, Type callbackType)
        : base($"Callback named '{callbackName}' of type '{callbackType.Name}' not found")
    {
        CallbackName = callbackName;
        CallbackType = callbackType;
    }
}











