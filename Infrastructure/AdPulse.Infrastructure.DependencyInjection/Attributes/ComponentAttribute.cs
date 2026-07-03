namespace AdPulse.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// ����������
/// ������ʽ�����Ҫ�Զ�ע������
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ComponentAttribute : Attribute
{
    /// <summary>
    /// ������ȼ�
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// �Զ������ý�·��
    /// </summary>
    public string? ConfigurationSection { get; set; }

    /// <summary>
    /// �����ǩ
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// �������
    /// </summary>
    public string Description { get; set; } = "";
}