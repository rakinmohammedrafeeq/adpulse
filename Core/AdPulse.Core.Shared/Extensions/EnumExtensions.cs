using System.ComponentModel;
using System.Reflection;

namespace AdPulse.Core.Shared.Extensions;

/// <summary>
/// ö����չ����
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// ��ȡö�ٵ�������Ϣ
    /// </summary>
    /// <param name="enumValue">ö��ֵ</param>
    /// <returns>������Ϣ</returns>
    public static string GetDescription(this Enum enumValue)
    {
        if (enumValue == null)
            return string.Empty;

        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null)
            return enumValue.ToString();

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// ��ȡö�ٵ���ʾ����
    /// </summary>
    /// <param name="enumValue">ö��ֵ</param>
    /// <returns>��ʾ����</returns>
    public static string GetDisplayName(this Enum enumValue)
    {
        if (enumValue == null)
            return string.Empty;

        var field = enumValue.GetType().GetField(enumValue.ToString());
        if (field == null)
            return enumValue.ToString();

        var attribute = field.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        return attribute?.Name ?? GetDescription(enumValue);
    }

    /// <summary>
    /// �ж�ö��ֵ�Ƿ���Ч
    /// </summary>
    /// <typeparam name="T">ö������</typeparam>
    /// <param name="enumValue">ö��ֵ</param>
    /// <returns>�Ƿ���Ч</returns>
    public static bool IsValid<T>(this T enumValue) where T : struct, Enum
    {
        return Enum.IsDefined(typeof(T), enumValue);
    }

    /// <summary>
    /// ���ַ���ת��Ϊö��ֵ
    /// </summary>
    /// <typeparam name="T">ö������</typeparam>
    /// <param name="value">�ַ���ֵ</param>
    /// <param name="ignoreCase">�Ƿ���Դ�Сд</param>
    /// <returns>ö��ֵ</returns>
    public static T? ParseEnum<T>(this string value, bool ignoreCase = true) where T : struct, Enum
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (Enum.TryParse<T>(value, ignoreCase, out var result))
            return result;

        return null;
    }

    /// <summary>
    /// ��ȡö�ٵ�����ֵ
    /// </summary>
    /// <typeparam name="T">ö������</typeparam>
    /// <returns>ö��ֵ�б�</returns>
    public static IEnumerable<T> GetValues<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>();
    }

    /// <summary>
    /// ��ȡö�ٵ����ƺ�ֵ���ֵ�
    /// </summary>
    /// <typeparam name="T">ö������</typeparam>
    /// <returns>���ƺ�ֵ���ֵ�</returns>
    public static IDictionary<string, T> GetNameValuePairs<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().ToDictionary(e => e.ToString(), e => e);
    }

    /// <summary>
    /// ��ȡö�ٵ�������ֵ���ֵ�
    /// </summary>
    /// <typeparam name="T">ö������</typeparam>
    /// <returns>������ֵ���ֵ�</returns>
    public static IDictionary<string, T> GetDescriptionValuePairs<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().ToDictionary(e => e.GetDescription(), e => e);
    }
}