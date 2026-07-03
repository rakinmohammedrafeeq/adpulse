namespace AdPulse.Infrastructure.Common.Models;

/// <summary>
/// ������ʩ����ѡ��
/// </summary>
public class InfrastructureOptions
{
    /// <summary>
    /// �Ƿ������Զ��������
    /// </summary>
    public bool EnableAutoDiscovery { get; set; } = true;
    
    /// <summary>
    /// �Ƿ����ý������
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;
    
    /// <summary>
    /// �Ƿ��������ܼ��
    /// </summary>
    public bool EnablePerformanceMonitoring { get; set; } = true;
    
    /// <summary>
    /// �Ƿ���������������
    /// </summary>
    public bool EnableConfigurationHotReload { get; set; } = true;
    
    /// <summary>
    /// ���ɨ��ĳ�������ģʽ
    /// </summary>
    public List<string> AssemblyPatterns { get; set; } = new() { "AdPulse.*" };
    
    /// <summary>
    /// �ų��ĳ�������ģʽ
    /// </summary>
    public List<string> ExcludedAssemblyPatterns { get; set; } = new();
    
    /// <summary>
    /// ������֤��ʱʱ�䣨�룩
    /// </summary>
    public int ConfigurationValidationTimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// �����ʼ����ʱʱ�䣨�룩
    /// </summary>
    public int ComponentInitializationTimeoutSeconds { get; set; } = 60;
}