namespace AdPulse.Core.Shared.Extensions;

/// <summary>
/// �ַ�����չ����
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// �ж��ַ����Ƿ�Ϊ�ջ�հ��ַ�
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <returns>�Ƿ�Ϊ�ջ�հ�</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// �ж��ַ����Ƿ�Ϊ���Ҳ�Ϊ�հ��ַ�
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <returns>�Ƿ���ֵ</returns>
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// ��ȫ��ȡ�ַ���
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <param name="maxLength">��󳤶�</param>
    /// <param name="suffix">����ʱ�ĺ�׺</param>
    /// <returns>��ȡ����ַ���</returns>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Length <= maxLength)
            return value;

        var truncateLength = Math.Max(0, maxLength - suffix.Length);
        return value[..truncateLength] + suffix;
    }

    /// <summary>
    /// ת��Ϊ��˹�������� (PascalCase)
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <returns>��˹���������ַ���</returns>
    public static string ToPascalCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var words = value.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var result = string.Join("", words.Select(word => 
            char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
        
        return result;
    }

    /// <summary>
    /// ת��Ϊ�շ������� (camelCase)
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <returns>�շ��������ַ���</returns>
    public static string ToCamelCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var pascalCase = value.ToPascalCase();
        if (pascalCase.Length == 0)
            return pascalCase;

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase[1..];
    }

    /// <summary>
    /// ת��Ϊ�»��������� (snake_case)
    /// </summary>
    /// <param name="value">�ַ���ֵ</param>
    /// <returns>�»����������ַ���</returns>
    public static string ToSnakeCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = System.Text.RegularExpressions.Regex.Replace(
            value, 
            @"([a-z])([A-Z])", 
            "$1_$2");
        
        return result.ToLowerInvariant();
    }

    /// <summary>
    /// �Ƴ��ַ����е�HTML��ǩ
    /// </summary>
    /// <param name="value">����HTML���ַ���</param>
    /// <returns>���ı��ַ���</returns>
    public static string StripHtml(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return System.Text.RegularExpressions.Regex.Replace(value, "<.*?>", string.Empty);
    }

    /// <summary>
    /// ���������Ϣ
    /// </summary>
    /// <param name="value">ԭʼ�ַ���</param>
    /// <param name="visibleLength">�ɼ��ַ�����</param>
    /// <param name="maskChar">�����ַ�</param>
    /// <returns>�������ַ���</returns>
    public static string Mask(this string? value, int visibleLength = 4, char maskChar = '*')
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        if (value.Length <= visibleLength)
            return new string(maskChar, value.Length);

        var visiblePart = value[..visibleLength];
        var maskPart = new string(maskChar, value.Length - visibleLength);
        
        return visiblePart + maskPart;
    }
}