using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AdPulse.Infrastructure.Common.Extensions;
using System.Reflection;

namespace AdPulse.Infrastructure.Configuration;

/// <summary>
/// ���ϵͳ���ù�����
/// �ṩͳһ�����ù�����Զ��󶨹���
/// </summary>
public class AdSystemConfigurationManager
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly ConfigurationValidationManager _validationManager;
    private readonly HashSet<Type> _registeredOptionsTypes = new();
    private readonly Dictionary<string, Type> _namedOptionsRegistry = new();

    public AdSystemConfigurationManager(IConfiguration configuration, IServiceCollection services)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _validationManager = new ConfigurationValidationManager(services);
    }

    /// <summary>
    /// �Զ�ע������ *Options ���͵�����
    /// </summary>
    public void RegisterAllOptions()
    {
        var optionsTypes = FindOptionsTypes();
        
        foreach (var optionsType in optionsTypes)
        {
            RegisterConventionOptions(optionsType);
        }
    }

    /// <summary>
    /// Ϊ���ע���ض����ã�֧������ѡ�
    /// </summary>
    /// <param name="componentType">�������</param>
    /// <param name="optionsType">����ѡ������</param>
    /// <param name="configurationPath">����·��</param>
    public void RegisterComponentOptions(Type componentType, Type optionsType, string configurationPath)
    {
        if (componentType == null) throw new ArgumentNullException(nameof(componentType));
        if (optionsType == null) throw new ArgumentNullException(nameof(optionsType));
        if (string.IsNullOrEmpty(configurationPath)) throw new ArgumentException("Configuration path cannot be null or empty", nameof(configurationPath));

        var componentName = componentType.Name;
        var registryKey = $"{optionsType.FullName}:{componentName}";

        // �����ظ�ע����ͬ����������
        if (_namedOptionsRegistry.ContainsKey(registryKey))
        {
            return;
        }

        // ����֧������ѡ��� Configure ����
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == "Configure" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 3 &&
                m.GetParameters()[1].ParameterType == typeof(string) &&
                m.GetParameters()[2].ParameterType == typeof(IConfiguration));

        if (configureMethod != null)
        {
            var genericMethod = configureMethod.MakeGenericMethod(optionsType);
            var configSection = _configuration.GetSection(configurationPath);

            // ʹ���������Ϊ����ѡ������ƣ�֧�ֶ������������ѡ������
            genericMethod.Invoke(null, new object[]
            {
                _services,
                componentName, // �������Ϊ����ѡ������
                configSection
            });

            // ��¼��ע�����������
            _namedOptionsRegistry[registryKey] = optionsType;
        }

        // ʹ��ͳһ����֤������
        _validationManager.RegisterValidatorsForOptionsType(optionsType);
    }

    /// <summary>
    /// ����Լ���Զ������ã��������ͳ�����������ã�
    /// </summary>
    /// <param name="optionsType">����ѡ������</param>
    private void RegisterConventionOptions(Type optionsType)
    {
        // �����ظ�ע��
        if (_registeredOptionsTypes.Contains(optionsType))
        {
            return;
        }

        // Լ����AdEngineOptions -> "AdEngine" ���ý�
        var sectionName = GetConfigurationSectionName(optionsType);

        // ʹ�� Microsoft.Extensions.Options ����ǿ���Ͱ�
        var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
            .GetMethods()
            .FirstOrDefault(m => 
                m.Name == "Configure" && 
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType == typeof(IServiceCollection) &&
                m.GetParameters()[1].ParameterType == typeof(IConfiguration));

        if (configureMethod != null)
        {
            var genericMethod = configureMethod.MakeGenericMethod(optionsType);
            genericMethod.Invoke(null, new object[]
            {
                _services,
                _configuration.GetSection(sectionName)
            });
        }

        // ��¼��ע�����������
        _registeredOptionsTypes.Add(optionsType);

        // ʹ��ͳһ����֤������
        _validationManager.RegisterValidatorsForOptionsType(optionsType);
    }

    /// <summary>
    /// �������� Options ����
    /// </summary>
    /// <returns>Options �����б�</returns>
    private IEnumerable<Type> FindOptionsTypes()
    {
        var assemblies = GetRelevantAssemblies();
        var optionsTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(t => t.Name.EndsWith("Options") && t.IsClass && !t.IsAbstract);
                optionsTypes.AddRange(types);
            }
            catch (Exception)
            {
                // ���Գ��򼯼��ش���
            }
        }

        return optionsTypes;
    }

    /// <summary>
    /// ��ȡ��صĳ���
    /// </summary>
    /// <returns>�����б�</returns>
    private IEnumerable<Assembly> GetRelevantAssemblies()
    {
        var assemblies = new List<Assembly>();
        
        // ��ӵ�ǰ����
        assemblies.Add(Assembly.GetExecutingAssembly());
        
        // ��ӵ��ó���
        var callingAssembly = Assembly.GetCallingAssembly();
        if (callingAssembly != null)
        {
            assemblies.Add(callingAssembly);
        }
        
        // �����ڳ���
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly != null)
        {
            assemblies.Add(entryAssembly);
        }

        // ����Ѽ��ص���س���
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName?.StartsWith("AdPulse") == true);
        assemblies.AddRange(loadedAssemblies);

        return assemblies.Distinct();
    }

    /// <summary>
    /// ���������������ƻ�ȡ���ý�����
    /// </summary>
    /// <param name="optionsType">��������</param>
    /// <returns>���ý�����</returns>
    private string GetConfigurationSectionName(Type optionsType)
    {
        var typeName = optionsType.Name;
        
        if (typeName.EndsWith("Options"))
        {
            return typeName.Substring(0, typeName.Length - "Options".Length);
        }
        
        if (typeName.EndsWith("Settings"))
        {
            return typeName.Substring(0, typeName.Length - "Settings".Length);
        }
        
        if (typeName.EndsWith("Configuration"))
        {
            return typeName.Substring(0, typeName.Length - "Configuration".Length);
        }
        
        return typeName;
    }
}