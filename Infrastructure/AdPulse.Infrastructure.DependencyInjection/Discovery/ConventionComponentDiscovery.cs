using AdPulse.Infrastructure.Common.Models;
using AdPulse.Infrastructure.Common.Conventions;
using AdPulse.Infrastructure.Common.Extensions;
using AdPulse.Infrastructure.DependencyInjection.Attributes;

namespace AdPulse.Infrastructure.DependencyInjection.Discovery;

/// <summary>
/// Լ�����������
/// ����Լ�������ֺ�ʶ�����
/// </summary>
public class ConventionComponentDiscovery
{
    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="type">�������</param>
    /// <returns>����������������������򷵻�null</returns>
    public ComponentDescriptor? DiscoverComponent(Type type)
    {
        if (!IsValidComponentType(type))
            return null;

        var descriptor = new ComponentDescriptor
        {
            ImplementationType = type,
            ServiceType = DetermineServiceType(type),
            Name = type.Name,
            Lifetime = DetermineLifetime(type),
            ConfigurationPath = DetermineConfigurationPath(type),
            ConfigurationType = DetermineConfigurationType(type),
            IsEnabled = DetermineIsEnabled(type),
            Priority = DeterminePriority(type)
        };

        // ���Ԫ����
        PopulateMetadata(descriptor, type);

        return descriptor;
    }

    /// <summary>
    /// �����������������
    /// </summary>
    /// <param name="types">�����б�</param>
    /// <returns>����������б�</returns>
    public IEnumerable<ComponentDescriptor> DiscoverComponents(IEnumerable<Type> types)
    {
        var descriptors = new List<ComponentDescriptor>();

        foreach (var type in types)
        {
            var descriptor = DiscoverComponent(type);
            if (descriptor != null)
            {
                descriptors.Add(descriptor);
            }
        }

        return descriptors;
    }

    /// <summary>
    /// ��������Ƿ�Ϊ��Ч���������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ���Ч</returns>
    private bool IsValidComponentType(Type type)
    {
        if (type == null || !type.IsConcreteClass())
            return false;

        // ����Ƿ�����ʽ��������
        if (type.GetCustomAttributeSafe<ComponentAttribute>() != null)
            return true;

        // ����Ƿ����Լ������
        return ComponentConventions.IsComponent(type);
    }

    /// <summary>
    /// ȷ����������
    /// </summary>
    /// <param name="type">ʵ������</param>
    /// <returns>��������</returns>
    private Type DetermineServiceType(Type type)
    {
        // ���Ȳ��Ҷ�Ӧ�Ľӿ�
        var interfaceName = $"I{type.Name}";
        var interfaceType = type.GetInterfaces()
            .FirstOrDefault(i => i.Name == interfaceName);

        if (interfaceType != null)
            return interfaceType;

        // �����������ܵķ���ӿ�
        var serviceInterfaces = type.GetInterfaces()
            .Where(i => !i.Name.StartsWith("System.") && 
                       !i.Name.StartsWith("Microsoft.") &&
                       i.IsPublic)
            .ToList();

        // ���ֻ��һ��ҵ��ӿڣ�ʹ����
        if (serviceInterfaces.Count == 1)
            return serviceInterfaces[0];

        // ����ʹ��ʵ�����ͱ���
        return type;
    }

    /// <summary>
    /// ȷ����������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>��������</returns>
    private ServiceLifetime DetermineLifetime(Type type)
    {
        // �����ʽ��������������
        var lifetimeAttribute = type.GetCustomAttributeSafe<ServiceLifetimeAttribute>();
        if (lifetimeAttribute != null)
            return lifetimeAttribute.Lifetime;

        // ʹ��Լ������
        return ComponentConventions.GetLifetimeByType(type);
    }

    /// <summary>
    /// ȷ������·��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>����·��</returns>
    private string? DetermineConfigurationPath(Type type)
    {
        // �����ʽ�����ð�����
        var bindingAttribute = type.GetCustomAttributeSafe<ConfigurationBindingAttribute>();
        if (bindingAttribute != null)
            return bindingAttribute.ConfigurationPath;

        // ʹ��Լ������
        return ComponentConventions.GetConfigurationPath(type);
    }

    /// <summary>
    /// ȷ����������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>��������</returns>
    private Type? DetermineConfigurationType(Type type)
    {
        // �����ʽ�����ð�����
        var bindingAttribute = type.GetCustomAttributeSafe<ConfigurationBindingAttribute>();
        if (bindingAttribute?.OptionsType != null)
            return bindingAttribute.OptionsType;

        // ����Լ��������������
        var optionsTypeName = $"{type.Name}Options";
        var optionsType = type.Assembly.GetTypes()
            .FirstOrDefault(t => t.Name == optionsTypeName && t.IsClass);

        return optionsType;
    }

    /// <summary>
    /// ȷ���Ƿ�����
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ�����</returns>
    private bool DetermineIsEnabled(Type type)
    {
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        return componentAttribute?.IsEnabled ?? true;
    }

    /// <summary>
    /// ȷ�����ȼ�
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>���ȼ�</returns>
    private int DeterminePriority(Type type)
    {
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        return componentAttribute?.Priority ?? 0;
    }

    /// <summary>
    /// ���Ԫ����
    /// </summary>
    /// <param name="descriptor">���������</param>
    /// <param name="type">����</param>
    private void PopulateMetadata(ComponentDescriptor descriptor, Type type)
    {
        // ��ӻ���Ԫ����
        descriptor.Metadata["FullName"] = type.FullName ?? type.Name;
        descriptor.Metadata["AssemblyName"] = type.Assembly.GetName().Name ?? "";

        // ����������Ԫ����
        var componentAttribute = type.GetCustomAttributeSafe<ComponentAttribute>();
        if (componentAttribute != null)
        {
            descriptor.Metadata["Description"] = componentAttribute.Description;
            descriptor.Metadata["Tags"] = componentAttribute.Tags;
        }

        // ���������Ϣ
        var dependencies = type.GetCustomAttributesSafe<DependencyAttribute>()
            .Select(d => new
            {
                ServiceType = d.ServiceType.Name,
                IsOptional = d.IsOptional,
                Description = d.Description
            })
            .ToList();

        if (dependencies.Any())
        {
            descriptor.Metadata["Dependencies"] = dependencies;
        }

        // ��ӽӿ���Ϣ
        var interfaces = type.GetInterfaces()
            .Where(i => !i.Name.StartsWith("System.") && !i.Name.StartsWith("Microsoft."))
            .Select(i => i.Name)
            .ToList();

        if (interfaces.Any())
        {
            descriptor.Metadata["Interfaces"] = interfaces;
        }
    }
}