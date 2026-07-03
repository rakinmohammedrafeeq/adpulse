using AdPulse.Infrastructure.Common.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdPulse.Infrastructure.Common.Base;

/// <summary>
/// �������������
/// �ṩ���ù���Ļ���ʵ��
/// </summary>
public abstract class ConfigurableComponentBase : IConfigurable
{
    /// <summary>
    /// ��������
    /// </summary>
    public abstract Type ConfigurationType { get; }
    
    /// <summary>
    /// �������
    /// </summary>
    /// <param name="configuration">���ö���</param>
    public virtual void Configure(object configuration)
    {
        if (configuration?.GetType() != ConfigurationType)
        {
            throw new ArgumentException($"Invalid configuration type. Expected: {ConfigurationType.Name}, Actual: {configuration?.GetType().Name}");
        }
        
        OnConfigurationChanged(configuration);
    }
    
    /// <summary>
    /// ���ñ��ʱ�Ĵ����߼���������ʵ��
    /// </summary>
    /// <param name="configuration">�µ����ö���</param>
    protected abstract void OnConfigurationChanged(object configuration);
}