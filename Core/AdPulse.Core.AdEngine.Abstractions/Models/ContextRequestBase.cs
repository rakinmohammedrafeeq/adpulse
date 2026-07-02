using AdPulse.Core.AdEngine.Abstractions.Enums;
using AdPulse.Core.AdEngine.Abstractions.Interfaces;
using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Shared.Entities;

namespace AdPulse.Core.AdEngine.Abstractions.Models;

/// <summary>
/// �������������ʵ����
/// </summary>
/// <typeparam name="T">����������</typeparam>
public abstract class ContextRequestBase<T> : IContextRequest<T> where T : class, ITargetingContext
{
    private readonly Dictionary<string, object> _parameters = new();

    /// <summary>
    /// ���������ͱ�ʶ
    /// </summary>
    public abstract string ContextType { get; }

    /// <summary>
    /// ��������ֵ�
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters => _parameters.AsReadOnly();

    /// <summary>
    /// ����ʱʱ��
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// �����������
    /// </summary>
    public CachePolicy CachePolicy { get; init; } = CachePolicy.Default;

    /// <summary>
    /// ����Ψһ��ʶ��
    /// </summary>
    public string RequestId { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// �������ȼ�
    /// </summary>
    public RequestPriority Priority { get; init; } = RequestPriority.Normal;

    /// <summary>
    /// �Ƿ�����ʹ�û�������
    /// </summary>
    public bool AllowCached { get; init; } = true;

    /// <summary>
    /// ���ɽ��ܵ������ӳ�ʱ��
    /// </summary>
    public TimeSpan MaxDataAge { get; init; } = TimeSpan.FromMinutes(10);

    /// <summary>
    /// ���캯��
    /// </summary>
    protected ContextRequestBase()
    {
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="parameters">��ʼ����</param>
    protected ContextRequestBase(IReadOnlyDictionary<string, object>? parameters)
    {
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                _parameters[kvp.Key] = kvp.Value;
            }
        }
    }

    /// <summary>
    /// ��ȡǿ���͵Ĳ���ֵ
    /// </summary>
    public virtual TValue? GetParameter<TValue>(string key)
    {
        if (!_parameters.TryGetValue(key, out var value))
            return default;

        if (value is TValue typedValue)
            return typedValue;

        // ��������ת��
        try
        {
            return (TValue?)Convert.ChangeType(value, typeof(TValue));
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// ����Ƿ����ָ���Ĳ���
    /// </summary>
    public virtual bool HasParameter(string key)
    {
        return _parameters.ContainsKey(key);
    }

    /// <summary>
    /// ���ò���ֵ
    /// </summary>
    protected void SetParameter(string key, object value)
    {
        _parameters[key] = value;
    }

    /// <summary>
    /// �������ò���
    /// </summary>
    protected void SetParameters(IReadOnlyDictionary<string, object> parameters)
    {
        foreach (var kvp in parameters)
        {
            _parameters[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// ��֤�����������Ч��
    /// </summary>
    public virtual ValidationResult Validate()
    {
        var errors = new List<ValidationError>();

        // ��֤��������
        if (string.IsNullOrWhiteSpace(ContextType))
        {
            errors.Add(new ValidationError
            {
                Message = "ContextType cannot be null or empty",
                Code = "CONTEXT_TYPE_REQUIRED",
                Field = nameof(ContextType)
            });
        }

        if (string.IsNullOrWhiteSpace(RequestId))
        {
            errors.Add(new ValidationError
            {
                Message = "RequestId cannot be null or empty",
                Code = "REQUEST_ID_REQUIRED",
                Field = nameof(RequestId)
            });
        }

        if (Timeout <= TimeSpan.Zero)
        {
            errors.Add(new ValidationError
            {
                Message = "Timeout must be greater than zero",
                Code = "INVALID_TIMEOUT",
                Field = nameof(Timeout),
                AttemptedValue = Timeout
            });
        }

        if (MaxDataAge < TimeSpan.Zero)
        {
            errors.Add(new ValidationError
            {
                Message = "MaxDataAge cannot be negative",
                Code = "INVALID_MAX_DATA_AGE",
                Field = nameof(MaxDataAge),
                AttemptedValue = MaxDataAge
            });
        }

        // ����������Զ�����֤
        ValidateSpecific(errors);

        return errors.Count == 0
            ? ValidationResult.Success("Request validation passed")
            : ValidationResult.Failure(errors, "Request validation failed");
    }

    /// <summary>
    /// �����ض�����֤�߼�
    /// </summary>
    protected virtual void ValidateSpecific(List<ValidationError> errors)
    {
        // ���������д�˷���������ض�����֤�߼�
    }

    /// <summary>
    /// ��ȡ����ժҪ��Ϣ
    /// </summary>
    public virtual string GetSummary()
    {
        return $"{ContextType} request {RequestId} with {_parameters.Count} parameters";
    }

    /// <summary>
    /// ��¡�������
    /// </summary>
    public virtual IContextRequest<T> Clone()
    {
        var clone = (ContextRequestBase<T>)MemberwiseClone();
        clone._parameters.Clear();
        foreach (var kvp in _parameters)
        {
            clone._parameters[kvp.Key] = kvp.Value;
        }
        return clone;
    }
}