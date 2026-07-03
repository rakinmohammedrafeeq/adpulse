using System.ComponentModel.DataAnnotations;

namespace AdPulse.Infrastructure.Common.Extensions;

/// <summary>
/// ��֤��չ����
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// ��֤�����Ƿ��������ע����֤����
    /// </summary>
    /// <param name="obj">Ҫ��֤�Ķ���</param>
    /// <returns>��֤���</returns>
    public static AdPulse.Core.Shared.Entities.ValidationResult ValidateObject(this object obj)
    {
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);

        return AdPulse.Core.Shared.Entities.ValidationResult.Create(isValid, results.Select(r => r.ErrorMessage ?? "Unknown error").ToList());

    }

    /// <summary>
    /// ��֤��������ֵ
    /// </summary>
    /// <param name="obj">Ҫ��֤�Ķ���</param>
    /// <param name="propertyName">��������</param>
    /// <param name="value">����ֵ</param>
    /// <returns>��֤���</returns>
    public static AdPulse.Core.Shared.Entities.ValidationResult ValidateProperty(this object obj, string propertyName, object? value)
    {
        var context = new ValidationContext(obj) { MemberName = propertyName };
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateProperty(value, context, results);

        return AdPulse.Core.Shared.Entities.ValidationResult.Create(isValid, results.Select(r => r.ErrorMessage ?? "Unknown error").ToList());
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    /// <param name="objects">Ҫ��֤�Ķ����б�</param>
    /// <returns>��֤����ֵ�</returns>
    public static Dictionary<object, AdPulse.Core.Shared.Entities.ValidationResult> ValidateObjects(this IEnumerable<object> objects)
    {
        var results = new Dictionary<object, AdPulse.Core.Shared.Entities.ValidationResult>();

        foreach (var obj in objects)
        {
            results[obj] = obj.ValidateObject();
        }

        return results;
    }

    /// <summary>
    /// �����֤����Ƿ�ȫ����Ч
    /// </summary>
    /// <param name="validationResults">��֤����ֵ�</param>
    /// <returns>�Ƿ�ȫ����Ч</returns>
    public static bool AllValid(this Dictionary<object, AdPulse.Core.Shared.Entities.ValidationResult> validationResults)
    {
        return validationResults.Values.All(r => r.IsValid);
    }

    /// <summary>
    /// ��ȡ������֤������Ϣ
    /// </summary>
    /// <param name="validationResults">��֤����ֵ�</param>
    /// <returns>������Ϣ�б�</returns>
    public static List<string> GetAllErrors(this Dictionary<object, AdPulse.Core.Shared.Entities.ValidationResult> validationResults)
    {
        var errors = new List<string>();

        foreach (var kvp in validationResults)
        {
            if (!kvp.Value.IsValid)
            {
                errors.AddRange(kvp.Value.Errors.Select(e => e.Message));
            }
        }

        return errors;
    }
}