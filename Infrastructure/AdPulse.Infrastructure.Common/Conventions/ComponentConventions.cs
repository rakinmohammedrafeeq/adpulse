using AdPulse.Infrastructure.Common.Models;

namespace AdPulse.Infrastructure.Common.Conventions;

/// <summary>
/// ���Լ������
/// </summary>
public class ComponentConventionRule
{
    /// <summary>
    /// ������ƺ�׺
    /// </summary>
    public string Suffix { get; set; } = "";
    
    /// <summary>
    /// �����������
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
    
    /// <summary>
    /// Ҫ��Ľӿ������б�
    /// </summary>
    public string[] RequiredInterfaceNames { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// ����·��ģ��
    /// </summary>
    public string ConfigurationPathTemplate { get; set; } = "";
    
    /// <summary>
    /// ����
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// �Ƿ�Ҫ���ض��ӿ�
    /// </summary>
    public bool RequireSpecificInterface { get; set; } = false;
}

/// <summary>
/// ���Լ���淶
/// </summary>
public static class ComponentConventions
{
    private static readonly List<ComponentConventionRule> _conventionRules = new()
    {
        new ComponentConventionRule
        {
            Suffix = "Strategy",
            Lifetime = ServiceLifetime.Transient,
            RequiredInterfaceNames = new[] { "IAdProcessingStrategy" },
            ConfigurationPathTemplate = "Strategies:{0}",
            Description = "��洦��������",
            RequireSpecificInterface = true
        },
        new ComponentConventionRule
        {
            Suffix = "Service",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = Array.Empty<string>(),
            ConfigurationPathTemplate = "Services:{0}",
            Description = "ҵ��������",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Manager",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = Array.Empty<string>(),
            ConfigurationPathTemplate = "Managers:{0}",
            Description = "���������",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Provider",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = new[] { "IDataAccessProvider", "IProvider" },
            ConfigurationPathTemplate = "DataProviders:{0}",
            Description = "�����ṩ�����",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "CallbackProvider",
            Lifetime = ServiceLifetime.Singleton,
            RequiredInterfaceNames = new[] { "ICallbackProvider" },
            ConfigurationPathTemplate = "CallbackProviders:{0}",
            Description = "�ص��ṩ�����",
            RequireSpecificInterface = true
        },
        new ComponentConventionRule
        {
            Suffix = "Matcher",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "ITargetingMatcher", "IMatcher" },
            ConfigurationPathTemplate = "Matchers:{0}",
            Description = "����ƥ�������",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Calculator",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "ICalculator" },
            ConfigurationPathTemplate = "Calculators:{0}",
            Description = "���������",
            RequireSpecificInterface = false
        },
        new ComponentConventionRule
        {
            Suffix = "Processor",
            Lifetime = ServiceLifetime.Scoped,
            RequiredInterfaceNames = new[] { "IProcessor" },
            ConfigurationPathTemplate = "Processors:{0}",
            Description = "���������",
            RequireSpecificInterface = false
        }
    };

    /// <summary>
    /// ��ȡ����Լ������
    /// </summary>
    /// <returns>Լ�������б�</returns>
    public static IReadOnlyList<ComponentConventionRule> GetAllRules() => _conventionRules.AsReadOnly();

    /// <summary>
    /// �������Ͳ���ƥ���Լ������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>ƥ���Լ���������û��ƥ���򷵻�null</returns>
    public static ComponentConventionRule? FindRuleByType(Type type)
    {
        // ���Ȼ�������Լ��ƥ��
        var rule = _conventionRules.FirstOrDefault(r => type.Name.EndsWith(r.Suffix));
        if (rule == null) return null;

        // ���Ҫ���ض��ӿڣ����нӿ���֤
        if (rule.RequireSpecificInterface && rule.RequiredInterfaceNames.Length > 0)
        {
            var implementedInterfaceNames = type.GetInterfaces().Select(i => i.Name).ToArray();
            var hasRequiredInterface = rule.RequiredInterfaceNames.Any(required =>
                implementedInterfaceNames.Contains(required));

            if (!hasRequiredInterface)
            {
                return null; // ������ӿ�Ҫ��
            }
        }

        return rule;
    }

    /// <summary>
    /// �������ͻ�ȡ��������
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>��������</returns>
    public static ServiceLifetime GetLifetimeByType(Type type)
    {
        var rule = FindRuleByType(type);
        return rule?.Lifetime ?? ServiceLifetime.Transient;
    }

    /// <summary>
    /// �������ͻ�ȡ����·��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>����·��</returns>
    public static string? GetConfigurationPath(Type type)
    {
        var rule = FindRuleByType(type);
        if (rule?.ConfigurationPathTemplate == null) return null;

        // ��ȡ�������
        var componentName = type.Name;
        if (componentName.EndsWith(rule.Suffix))
        {
            componentName = componentName.Substring(0, componentName.Length - rule.Suffix.Length);
        }

        return string.Format(rule.ConfigurationPathTemplate, componentName);
    }

    /// <summary>
    /// ����µ�Լ������
    /// </summary>
    /// <param name="rule">Լ������</param>
    public static void AddConventionRule(ComponentConventionRule rule)
    {
        if (rule == null) throw new ArgumentNullException(nameof(rule));
        if (string.IsNullOrEmpty(rule.Suffix)) throw new ArgumentException("Suffix cannot be null or empty");

        // ����Ƿ��Ѵ�����ͬ��׺�Ĺ���
        if (_conventionRules.Any(r => r.Suffix == rule.Suffix))
        {
            throw new InvalidOperationException($"Convention rule with suffix '{rule.Suffix}' already exists");
        }

        _conventionRules.Add(rule);
    }

    /// <summary>
    /// ��������Ƿ�������Լ��
    /// </summary>
    /// <param name="type">����</param>
    /// <returns>�Ƿ����Լ��</returns>
    public static bool IsComponent(Type type)
    {
        if (type == null || !type.IsClass || type.IsAbstract || type.IsGenericTypeDefinition)
            return false;

        // ����Ƿ����Լ������
        return FindRuleByType(type) != null;
    }
}