using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AdPulse.Infrastructure.Common.Extensions;
using System.Reflection;

namespace AdPulse.Infrastructure.Configuration;

/// <summary>
/// ������֤������
/// �ṩͳһ��������֤����
/// </summary>
public class ConfigurationValidationManager
{
    private readonly IServiceCollection _services;
    private readonly HashSet<Type> _registeredValidators = new();

    public ConfigurationValidationManager(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <summary>
    /// �Զ�ɨ���ע��������֤��
    /// </summary>
    public void RegisterAllValidators()
    {
        var validatorTypes = FindValidatorTypes();
        
        foreach (var validatorType in validatorTypes)
        {
            RegisterValidatorType(validatorType);
        }
    }

    /// <summary>
    /// Ϊ�ض���������ע����֤��
    /// </summary>
    /// <param name="optionsType">����ѡ������</param>
    public void RegisterValidatorsForOptionsType(Type optionsType)
    {
        if (optionsType == null) throw new ArgumentNullException(nameof(optionsType));

        var validatorTypes = FindValidatorTypesForOptionsType(optionsType);
        
        foreach (var validatorType in validatorTypes)
        {
            RegisterValidatorType(validatorType);
        }
    }

    /// <summary>
    /// ������֤��ע���߼� - �����ظ�����
    /// </summary>
    /// <param name="validatorType">��֤������</param>
    private void RegisterValidatorType(Type validatorType)
    {
        if (_registeredValidators.Contains(validatorType))
        {
            return;
        }

        var validateInterface = validatorType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidateOptions<>));

        if (validateInterface != null)
        {
            var optionsType = validateInterface.GetGenericArguments()[0];
            var serviceType = typeof(IValidateOptions<>).MakeGenericType(optionsType);

            _services.AddSingleton(serviceType, validatorType);
            _registeredValidators.Add(validatorType);
        }
    }

    /// <summary>
    /// ����������֤������
    /// </summary>
    /// <returns>��֤�������б�</returns>
    private IEnumerable<Type> FindValidatorTypes()
    {
        var assemblies = GetRelevantAssemblies();
        var validatorTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(type => type.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidateOptions<>)));
                validatorTypes.AddRange(types);
            }
            catch (Exception)
            {
                // ���Գ��򼯼��ش���
            }
        }

        return validatorTypes;
    }

    /// <summary>
    /// �����ض��������͵���֤��
    /// </summary>
    /// <param name="optionsType">����ѡ������</param>
    /// <returns>��֤�������б�</returns>
    private IEnumerable<Type> FindValidatorTypesForOptionsType(Type optionsType)
    {
        var assemblies = GetRelevantAssemblies();
        var validatorTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetConcreteTypes()
                    .Where(type => type.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidateOptions<>) &&
                        i.GetGenericArguments()[0] == optionsType));
                validatorTypes.AddRange(types);
            }
            catch (Exception)
            {
                // ���Գ��򼯼��ش���
            }
        }

        return validatorTypes;
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
}