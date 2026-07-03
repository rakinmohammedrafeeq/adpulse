using Microsoft.Extensions.DependencyInjection;
using AdPulse.Infrastructure.Common.Models;
using AdPulse.Infrastructure.Configuration;
using AdPulse.Infrastructure.DependencyInjection.Discovery;
using AdPulse.Infrastructure.DependencyInjection.Attributes;
using AdPulse.Infrastructure.Common.Extensions;
using AdPulse.Infrastructure.Common.Conventions;
using CommonServiceLifetime = AdPulse.Infrastructure.Common.Models.ServiceLifetime;
using MicrosoftServiceLifetime = Microsoft.Extensions.DependencyInjection.ServiceLifetime;

namespace AdPulse.Infrastructure.DependencyInjection;

/// <summary>
/// ���ע�������
/// ����������Զ����ֺ�ע��
/// </summary>
public class ComponentRegistrationManager
{
    private readonly IServiceCollection _services;
    private readonly AdSystemConfigurationManager _configManager;
    private readonly ConventionComponentDiscovery _componentDiscovery;
    private readonly AssemblyComponentScanner _scanner;
    private readonly HashSet<Type> _registeredTypes = new();

    public ComponentRegistrationManager(IServiceCollection services, AdSystemConfigurationManager configManager)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        _componentDiscovery = new ConventionComponentDiscovery();
        _scanner = new AssemblyComponentScanner();
    }

    /// <summary>
    /// ע���������
    /// </summary>
    public void RegisterAllComponents()
    {
        // ����ɨ����
        ConfigureScanner();

        // ɨ���������
        var componentTypes = _scanner.ScanComponentTypes();

        // ���ֺ�ע�����
        var descriptors = _componentDiscovery.DiscoverComponents(componentTypes);
        
        foreach (var descriptor in descriptors.Where(d => d.IsEnabled))
        {
            RegisterComponent(descriptor);
        }
    }

    /// <summary>
    /// ע�ᵥ�����
    /// </summary>
    /// <param name="descriptor">���������</param>
    public void RegisterComponent(ComponentDescriptor descriptor)
    {
        if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
        
        // �����ظ�ע��
        if (_registeredTypes.Contains(descriptor.ImplementationType))
            return;

        try
        {
            // ע�ᵽ DI ����
            RegisterServiceToContainer(descriptor);

            // �������ð�
            ProcessConfigurationBinding(descriptor);

            // ��¼��ע�������
            _registeredTypes.Add(descriptor.ImplementationType);
        }
        catch (Exception ex)
        {
            // ��¼ע��ʧ�ܵ����������Ӱ�����������ע��
            System.Diagnostics.Debug.WriteLine($"Failed to register component {descriptor.ImplementationType.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// ����ɨ����
    /// </summary>
    private void ConfigureScanner()
    {
        // ���Ĭ�ϵĳ���ģʽ
        _scanner.AddAssemblyPattern("AdPulse.*");
        
        // �ų����Ժͻ�����ʩ����
        _scanner.ExcludeAssemblyPattern("*.Tests");
        _scanner.ExcludeAssemblyPattern("*.Test");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.Common");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.Configuration");
        _scanner.ExcludeAssemblyPattern("*.Infrastructure.DependencyInjection");
    }

    /// <summary>
    /// ע���������
    /// </summary>
    /// <param name="descriptor">���������</param>
    private void RegisterServiceToContainer(ComponentDescriptor descriptor)
    {
        var serviceLifetime = ConvertToMicrosoftLifetime(descriptor.Lifetime);
        
        // ��������������
        var serviceDescriptor = new ServiceDescriptor(
            descriptor.ServiceType,
            descriptor.ImplementationType,
            serviceLifetime);

        _services.Add(serviceDescriptor);

        // ����������ͺ�ʵ�����Ͳ�ͬ��Ҳע��ʵ�����ͱ���
        if (descriptor.ServiceType != descriptor.ImplementationType)
        {
            var implementationDescriptor = new ServiceDescriptor(
                descriptor.ImplementationType,
                descriptor.ImplementationType,
                serviceLifetime);

            _services.Add(implementationDescriptor);
        }
    }

    /// <summary>
    /// �������ð�
    /// </summary>
    /// <param name="descriptor">���������</param>
    private void ProcessConfigurationBinding(ComponentDescriptor descriptor)
    {
        if (descriptor.ConfigurationType == null || string.IsNullOrEmpty(descriptor.ConfigurationPath))
            return;

        try
        {
            // ע������ض�������ѡ��
            _configManager.RegisterComponentOptions(
                descriptor.ImplementationType,
                descriptor.ConfigurationType,
                descriptor.ConfigurationPath);
        }
        catch (Exception ex)
        {
            // ���ð�ʧ�ܲ�Ӱ�����ע��
            System.Diagnostics.Debug.WriteLine($"Failed to bind configuration for {descriptor.ImplementationType.Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// ת����������ö��
    /// </summary>
    /// <param name="lifetime">�Զ�����������</param>
    /// <returns>΢��DI��������</returns>
    private MicrosoftServiceLifetime ConvertToMicrosoftLifetime(CommonServiceLifetime lifetime)
    {
        return lifetime switch
        {
            CommonServiceLifetime.Singleton => MicrosoftServiceLifetime.Singleton,
            CommonServiceLifetime.Scoped => MicrosoftServiceLifetime.Scoped,
            CommonServiceLifetime.Transient => MicrosoftServiceLifetime.Transient,
            _ => MicrosoftServiceLifetime.Transient
        };
    }

    /// <summary>
    /// ��ȡ��ע����������
    /// </summary>
    /// <returns>��ע������ͼ���</returns>
    public IReadOnlySet<Type> GetRegisteredTypes()
    {
        return _registeredTypes.ToHashSet();
    }

    /// <summary>
    /// ��������Ƿ���ע��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ���ע��</returns>
    public bool IsRegistered(Type type)
    {
        return _registeredTypes.Contains(type);
    }

    /// <summary>
    /// �ֶ�ע���������
    /// </summary>
    /// <typeparam name="TImplementation">ʵ������</typeparam>
    /// <typeparam name="TService">��������</typeparam>
    /// <param name="lifetime">��������</param>
    /// <param name="configurationPath">����·��</param>
    public void RegisterComponent<TImplementation, TService>(
        CommonServiceLifetime lifetime = CommonServiceLifetime.Transient,
        string? configurationPath = null)
        where TImplementation : class, TService
        where TService : class
    {
        var descriptor = new ComponentDescriptor
        {
            ImplementationType = typeof(TImplementation),
            ServiceType = typeof(TService),
            Name = typeof(TImplementation).Name,
            Lifetime = lifetime,
            ConfigurationPath = configurationPath,
            IsEnabled = true,
            Priority = 0
        };

        // ���Բ�����������
        if (!string.IsNullOrEmpty(configurationPath))
        {
            var optionsTypeName = $"{typeof(TImplementation).Name}Options";
            descriptor.ConfigurationType = typeof(TImplementation).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name == optionsTypeName);
        }

        RegisterComponent(descriptor);
    }

    /// <summary>
    /// �ֶ�ע���������
    /// </summary>
    /// <typeparam name="TImplementation">ʵ������</typeparam>
    /// <param name="lifetime">��������</param>
    /// <param name="configurationPath">����·��</param>
    public void RegisterComponent<TImplementation>(
        CommonServiceLifetime lifetime = CommonServiceLifetime.Transient,
        string? configurationPath = null)
        where TImplementation : class
    {
        RegisterComponent<TImplementation, TImplementation>(lifetime, configurationPath);
    }
}