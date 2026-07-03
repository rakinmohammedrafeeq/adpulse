using AdPulse.Infrastructure.Common.Models;

namespace AdPulse.Infrastructure.Composition.Configuration;

/// <summary>
/// ������ʩ���ѡ��
/// </summary>
public class BootstrapOptions
{
    /// <summary>
    /// �Ƿ������Զ��������
    /// </summary>
    public bool EnableAutoComponentDiscovery { get; set; } = true;

    /// <summary>
    /// �Ƿ������Զ����ð�
    /// </summary>
    public bool EnableAutoConfigurationBinding { get; set; } = true;

    /// <summary>
    /// �Ƿ����ý������
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// �Ƿ�����������֤
    /// </summary>
    public bool EnableConfigurationValidation { get; set; } = true;

    /// <summary>
    /// ����ɨ��ģʽ
    /// </summary>
    public AssemblyScanMode AssemblyScanMode { get; set; } = AssemblyScanMode.LoadedAssemblies;

    /// <summary>
    /// Ҫɨ��ĳ�������ģʽ
    /// </summary>
    public List<string> AssemblyIncludePatterns { get; set; } = new() { "AdPulse.*" };

    /// <summary>
    /// Ҫ�ų��ĳ�������ģʽ
    /// </summary>
    public List<string> AssemblyExcludePatterns { get; set; } = new() { "*.Tests", "*.Test" };

    /// <summary>
    /// ���ע�ᳬʱʱ�䣨�룩
    /// </summary>
    public int ComponentRegistrationTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// ������֤��ʱʱ�䣨�룩
    /// </summary>
    public int ConfigurationValidationTimeoutSeconds { get; set; } = 15;

    /// <summary>
    /// �Ƿ������ʱ��֤�������
    /// </summary>
    public bool ValidateComponentsOnStartup { get; set; } = true;

    /// <summary>
    /// ʧ������ģʽ
    /// </summary>
    public FailureToleranceMode FailureToleranceMode { get; set; } = FailureToleranceMode.ContinueOnError;
}

/// <summary>
/// ����ɨ��ģʽ
/// </summary>
public enum AssemblyScanMode
{
    /// <summary>
    /// ��ɨ���Ѽ��صĳ���
    /// </summary>
    LoadedAssemblies,

    /// <summary>
    /// ɨ��Ӧ�ó���Ŀ¼�µ����г���
    /// </summary>
    ApplicationDirectory,

    /// <summary>
    /// �ֶ�ָ������
    /// </summary>
    Manual
}

/// <summary>
/// ʧ������ģʽ
/// </summary>
public enum FailureToleranceMode
{
    /// <summary>
    /// ��������ʱֹͣ���
    /// </summary>
    FailFast,

    /// <summary>
    /// ��������ʱ����������¼����
    /// </summary>
    ContinueOnError,

    /// <summary>
    /// �������д���
    /// </summary>
    IgnoreErrors
}