namespace AdPulse.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// ���ð�����
/// ������ʽָ����������ð���Ϣ
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConfigurationBindingAttribute : Attribute
{
    /// <summary>
    /// ����·��
    /// </summary>
    public string ConfigurationPath { get; }

    /// <summary>
    /// ����ѡ������
    /// </summary>
    public Type? OptionsType { get; set; }

    /// <summary>
    /// �Ƿ�ʹ������ѡ��
    /// </summary>
    public bool UseNamedOptions { get; set; } = false;

    /// <summary>
    /// ����ѡ�������
    /// </summary>
    public string? OptionsName { get; set; }

    /// <summary>
    /// ��ʼ�����ð�����
    /// </summary>
    /// <param name="configurationPath">����·��</param>
    public ConfigurationBindingAttribute(string configurationPath)
    {
        ConfigurationPath = configurationPath ?? throw new ArgumentNullException(nameof(configurationPath));
    }
}