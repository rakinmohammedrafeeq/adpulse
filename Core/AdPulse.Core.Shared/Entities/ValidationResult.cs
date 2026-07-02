namespace AdPulse.Core.Shared.Entities;

/// <summary>
/// ��֤���
/// </summary>
public sealed class ValidationResult : System.ComponentModel.DataAnnotations.ValidationResult
{
    private IReadOnlyList<ValidationError> errors = Array.Empty<ValidationError>();

    /// <summary>
    /// �Ƿ���֤�ɹ�
    /// </summary>
    public bool IsValid { get; init; }

    private IReadOnlyList<ValidationWarning> warnings = Array.Empty<ValidationWarning>();

    public ValidationResult(string? errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get => errors; init => errors = value?.ToList() ?? new List<ValidationError>(); }
    /// <summary>
    /// ������Ϣ�б�
    /// </summary>
    public IReadOnlyList<ValidationWarning> Warnings { get => warnings; init => warnings = value?.ToList() ?? new List<ValidationWarning>(); }

    /// <summary>
    /// ��֤ժҪ��Ϣ
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// ��֤��������Ϣ
    /// </summary>
    public IReadOnlyDictionary<string, object>? Context { get; init; }

    /// <summary>
    /// ��֤��ʱ
    /// </summary>
    public TimeSpan Duration { get; init; }

    /// <summary>
    /// �����ɹ�����֤���
    /// </summary>
    public static new ValidationResult Success(string? summary = null, TimeSpan duration = default)
    {
        return new ValidationResult(summary)
        {
            IsValid = true,
            Summary = summary,
            Duration = duration
        };
    }

    // Fix for CA1826: Use the collection directly instead of Enumerable methods
    public static ValidationResult Failure(
        IEnumerable<ValidationError> errors,
        string? summary = null,
        TimeSpan duration = default)
    {
        return new ValidationResult(summary)
        {
            IsValid = false,
            Errors = errors is List<ValidationError> errorList ? errorList.AsReadOnly() : errors.ToList().AsReadOnly(),
            Summary = summary,
            Duration = duration
        };
    }

    /// <summary>
    /// �������������֤���
    /// </summary>
    public static ValidationResult SuccessWithWarnings(
    IEnumerable<ValidationWarning> warnings,
    string? summary = null,
    TimeSpan duration = default)
    {
        return new ValidationResult(summary)
        {
            IsValid = true,
            Warnings = warnings is List<ValidationWarning> warningList ? warningList.AsReadOnly() : warnings.ToList().AsReadOnly(),
            Summary = summary,
            Duration = duration
        };
    }

    // Fix for CS1061: Ensure ValidationError has a Message property or use ErrorMessage
    public string? GetFirstErrorMessage()
    {
        return Errors.FirstOrDefault()?.Message; // Assuming ErrorMessage is the correct property
    }

    /// <summary>
    /// ��ȡ���д�����Ϣ
    /// </summary>
    public IEnumerable<string> GetAllErrorMessages()
    {
        return Errors.Select(e => e.Message); // Assuming Message is the correct property
    }

    /// <summary>
    /// ��ȡ���о�����Ϣ
    /// </summary>
    public IEnumerable<string> GetAllWarningMessages()
    {
        return Warnings.Select(w => w.Message);
    }

    public void AddError(string v)
    {
        // ��������Ϣ�Ƿ�Ϊ�ջ�հ�
        if (string.IsNullOrWhiteSpace(v))
        {
            throw new ArgumentException("������Ϣ����Ϊ��", nameof(v));
        }

        // ��������Ϣ��ӵ�Errors�б���  
        var errorsList = Errors.ToList();
        errorsList.Add(new ValidationError { Message = v }); // ʹ�ö����ʼֵ�趨������Message����  
        errors = errorsList.AsReadOnly();
    }

    public void AddWarning(string v)
    {
        if (string.IsNullOrWhiteSpace(v))
        {
            throw new ArgumentException("������Ϣ����Ϊ�ջ�հס�", nameof(v));
        }

        var warningsList = Warnings.ToList();
        warningsList.Add(new ValidationWarning { Message = v }); // ʹ�ö����ʼֵ�趨������Message����  
        warnings = warningsList.AsReadOnly();
    }

    public static ValidationResult Failure(List<string> errors)
    {
        // ��������Ϣת��ΪValidationError����
        var validationErrors = errors.Select(error => new ValidationError { Message = error }).ToList();
        // ����ValidationResult�����Ϊ��Ч������������Ϣ
        return new ValidationResult(errors.FirstOrDefault())
        {
            IsValid = false,
            Errors = validationErrors
        };
    }

    public static ValidationResult Create(bool isValid, List<string> list)
    {
        if (isValid)
        {
            return Success(null, TimeSpan.Zero);
        }
        else
        {
            var validationErrors = list.Select(error => new ValidationError { Message = error }).ToList();
            return ValidationResult.Failure(validationErrors, "Validation failed.", TimeSpan.Zero);
        }
    }
}



