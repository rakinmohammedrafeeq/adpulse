using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AdPulse.Infrastructure.Configuration;
using AdPulse.Infrastructure.DependencyInjection;
using AdPulse.Infrastructure.Composition.Configuration;
using AdPulse.Infrastructure.Common.Abstractions;

namespace AdPulse.Infrastructure.Composition.Bootstrapper;

/// <summary>
/// ������ʩ�����
/// ����Э������������ʩ��������
/// </summary>
public class InfrastructureBootstrapper
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly BootstrapOptions _options;
    private readonly List<string> _errors = new();
    private readonly List<string> _warnings = new();

    public InfrastructureBootstrapper(IServiceCollection services, IConfiguration configuration, BootstrapOptions? options = null)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _options = options ?? new BootstrapOptions();
    }

    /// <summary>
    /// ���������ʩ
    /// </summary>
    /// <returns>���񼯺�</returns>
    public IServiceCollection Bootstrap()
    {
        try
        {
            // 1. ���ù��� - �Զ�ע��������������
            if (_options.EnableAutoConfigurationBinding)
            {
                BootstrapConfiguration();
            }

            // 2. ���ע�� - ����Լ�������Զ�ע��
            if (_options.EnableAutoComponentDiscovery)
            {
                BootstrapComponents();
            }

            // 3. ������飨�Զ�Ϊ���������ӣ�
            if (_options.EnableHealthChecks)
            {
                BootstrapHealthChecks();
            }

            // 4. ������֤
            if (_options.EnableConfigurationValidation)
            {
                BootstrapValidation();
            }

            return _services;
        }
        catch (Exception ex)
        {
            _errors.Add($"Bootstrap failed: {ex.Message}");
            
            if (_options.FailureToleranceMode == FailureToleranceMode.FailFast)
            {
                throw new InvalidOperationException($"Infrastructure bootstrap failed: {ex.Message}", ex);
            }

            return _services;
        }
    }

    /// <summary>
    /// ������ù���
    /// </summary>
    private void BootstrapConfiguration()
    {
        try
        {
            var configManager = new AdSystemConfigurationManager(_configuration, _services);
            configManager.RegisterAllOptions();
        }
        catch (Exception ex)
        {
            HandleError("Configuration bootstrap failed", ex);
        }
    }

    /// <summary>
    /// ������ע��
    /// </summary>
    private void BootstrapComponents()
    {
        try
        {
            var configManager = new AdSystemConfigurationManager(_configuration, _services);
            var componentManager = new ComponentRegistrationManager(_services, configManager);
            componentManager.RegisterAllComponents();
        }
        catch (Exception ex)
        {
            HandleError("Component registration failed", ex);
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void BootstrapHealthChecks()
    {
        try
        {
            var healthChecksBuilder = _services.AddHealthChecks();
            
            // �Զ�Ϊ����ʵ�� IHealthCheckable �������ӽ������
            // ����򻯴����ʵ��ʵ���л�ɨ��������ע��ķ���
            healthChecksBuilder.AddCheck("infrastructure", () => HealthCheckResult.Healthy("Infrastructure is healthy"));
        }
        catch (Exception ex)
        {
            HandleError("Health checks bootstrap failed", ex);
        }
    }

    /// <summary>
    /// ���������֤
    /// </summary>
    private void BootstrapValidation()
    {
        try
        {
            var validationManager = new ConfigurationValidationManager(_services);
            validationManager.RegisterAllValidators();
        }
        catch (Exception ex)
        {
            HandleError("Validation bootstrap failed", ex);
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="message">������Ϣ</param>
    /// <param name="exception">�쳣</param>
    private void HandleError(string message, Exception exception)
    {
        var fullMessage = $"{message}: {exception.Message}";
        _errors.Add(fullMessage);

        if (_options.FailureToleranceMode == FailureToleranceMode.FailFast)
        {
            throw new InvalidOperationException(fullMessage, exception);
        }

        // ��¼���󵫼���ִ��
        System.Diagnostics.Debug.WriteLine($"[ERROR] {fullMessage}");
    }

    /// <summary>
    /// ��ȡ��������еĴ���
    /// </summary>
    /// <returns>�����б�</returns>
    public IReadOnlyList<string> GetErrors()
    {
        return _errors.AsReadOnly();
    }

    /// <summary>
    /// ��ȡ��������еľ���
    /// </summary>
    /// <returns>�����б�</returns>
    public IReadOnlyList<string> GetWarnings()
    {
        return _warnings.AsReadOnly();
    }

    /// <summary>
    /// �������Ƿ�ɹ�
    /// </summary>
    /// <returns>�Ƿ�ɹ�</returns>
    public bool IsSuccessful()
    {
        return !_errors.Any();
    }
}