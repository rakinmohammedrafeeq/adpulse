namespace AdPulse.Infrastructure.Common.Models;

/// <summary>
/// ��������������������Ԫ������Ϣ
/// </summary>
public class ComponentDescriptor
{
    /// <summary>
    /// ���ʵ������
    /// </summary>
    public Type ImplementationType { get; set; } = null!;
    
    /// <summary>
    /// ����������ͣ�ͨ���ǽӿ����ͣ�
    /// </summary>
    public Type ServiceType { get; set; } = null!;
    
    /// <summary>
    /// �������
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// �����������
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    
    /// <summary>
    /// ����·��
    /// </summary>
    public string? ConfigurationPath { get; set; }
    
    /// <summary>
    /// ��������
    /// </summary>
    public Type? ConfigurationType { get; set; }
    
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// ���ȼ�
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// ��չԪ����
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// ������������ö��
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// ˲̬��ÿ�����󶼴�����ʵ��
    /// </summary>
    Transient,
    
    /// <summary>
    /// ��������ͬһ��������ʹ��ͬһʵ��
    /// </summary>
    Scoped,
    
    /// <summary>
    /// ����������Ӧ�ó�������������ʹ��ͬһʵ��
    /// </summary>
    Singleton
}