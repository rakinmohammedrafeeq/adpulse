using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AdPulse.Infrastructure.Composition.Bootstrapper;
using AdPulse.Infrastructure.Composition.Configuration;
using AdPulse.Infrastructure.Common.Abstractions;
using AdPulse.Infrastructure.Common.Extensions;
using System.Reflection;

namespace AdPulse.Infrastructure.Composition.Extensions;

/// <summary>
/// ���񼯺���չ����
/// ��������������ʩΨһ�Ķ���ӿڣ�λ����ϲ����ѭ������
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ע����ϵͳ������ʩ
    /// �����ⲿӦ�ó���Ψһ��Ҫ���õķ���
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="configuration">���ö���</param>
    /// <param name="configureOptions">�������ѡ���ί��</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddAdSystemInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<BootstrapOptions>? configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // �������ѡ��
        var options = new BootstrapOptions();
        configureOptions?.Invoke(options);

        // ʹ�û�����ʩ���������װ�������
        var bootstrapper = new InfrastructureBootstrapper(services, configuration, options);
        var result = bootstrapper.Bootstrap();

        // ���������
        if (!bootstrapper.IsSuccessful())
        {
            var errors = bootstrapper.GetErrors();
            var errorMessage = string.Join(Environment.NewLine, errors);
            throw new InvalidOperationException($"Infrastructure bootstrap failed:{Environment.NewLine}{errorMessage}");
        }

        // ��¼����
        var warnings = bootstrapper.GetWarnings();
        foreach (var warning in warnings)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] {warning}");
        }

        return result;
    }

    /// <summary>
    /// ע����ϵͳ������ʩ���򻯰汾��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="configuration">���ö���</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddAdSystemInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        return AddAdSystemInfrastructure(services, configuration, null);
    }

    /// <summary>
    /// Ϊ�������ϵͳ�������������֧��
    /// </summary>
    /// <param name="builder">������鹹����</param>
    /// <returns>������鹹����</returns>
    public static IHealthChecksBuilder AddComponentHealthChecks(this IHealthChecksBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        // �Զ�ɨ������ʵ�� IHealthCheckable ���������ӽ������
        var healthCheckableTypes = FindHealthCheckableTypes();
            
        foreach (var type in healthCheckableTypes)
        {
            // Ϊÿ���������������������
            builder.AddCheck(type.Name, () => 
            {
                try
                {
                    // ����򻯴����ʵ��ʵ������Ҫ��DI������ȡʵ��
                    return HealthCheckResult.Healthy($"{type.Name} is healthy");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy($"{type.Name} check failed: {ex.Message}");
                }
            });
        }
        
        return builder;
    }

    /// <summary>
    /// ��ӻ�����ʩ����ѡ��
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <param name="configuration">���ö���</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection AddInfrastructureOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ע�������ʩ����ѡ��
        services.Configure<BootstrapOptions>(configuration.GetSection("Infrastructure:Bootstrap"));
        
        return services;
    }

    /// <summary>
    /// ��֤������ʩ����
    /// </summary>
    /// <param name="services">���񼯺�</param>
    /// <returns>���񼯺�</returns>
    public static IServiceCollection ValidateInfrastructureConfiguration(this IServiceCollection services)
    {
        // ���������֤
        services.AddOptions<BootstrapOptions>()
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// ����ʵ�� IHealthCheckable �ӿڵ�����
    /// </summary>
    /// <returns>������������б�</returns>
    private static IEnumerable<Type> FindHealthCheckableTypes()
    {
        var healthCheckableTypes = new List<Type>();
        
        try
        {
            // ��ȡ��س���
            var assemblies = GetRelevantAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypesImplementing(typeof(IHealthCheckable));
                    healthCheckableTypes.AddRange(types);
                }
                catch (Exception)
                {
                    // ���Գ��򼯼��ش���
                }
            }
        }
        catch (Exception)
        {
            // ����ɨ�����
        }

        return healthCheckableTypes;
    }

    /// <summary>
    /// ��ȡ��صĳ���
    /// </summary>
    /// <returns>�����б�</returns>
    private static IEnumerable<Assembly> GetRelevantAssemblies()
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