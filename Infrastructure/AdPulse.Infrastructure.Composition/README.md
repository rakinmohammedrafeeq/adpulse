# ͳһ���û�������ע��ܹ� - ʹ��ָ��

## ����

��������ʩ�ṩ��һ�������ġ����伴�õ����ù��������ע��������������Լ���������õ�ԭ��ʵ�������õ������չ������

## ���ٿ�ʼ

### 1. ��Ӧ�ó��������û�����ʩ

```csharp
// Program.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AdPulse.Infrastructure.Composition.Extensions;

public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // һ�������������ϵͳ������ʩ
                services.AddAdSystemInfrastructure(context.Configuration);
                
                // ��ѡ����ӽ������
                services.AddHealthChecks()
                    .AddComponentHealthChecks();
            })
            .Build();
            
        host.Run();
    }
}
```

### 2. ����ҵ�����

��ѭԼ����������������Զ������ֺ�ע�᣺

```csharp
// �������ʾ��
public class UserInterestRecallStrategy : IAdProcessingStrategy, IHealthCheckable
{
    private readonly UserInterestRecallOptions _options;
    private readonly ILogger<UserInterestRecallStrategy> _logger;

    // ���û��Զ�ע�루��������Լ����
    public UserInterestRecallStrategy(
        IOptionsMonitor<UserInterestRecallOptions> options,
        ILogger<UserInterestRecallStrategy> logger)
    {
        _options = options.CurrentValue;
        _logger = logger;
        
        // �������ñ���¼�
        options.OnChange(OnConfigurationChanged);
    }

    public async Task<ProcessingResult> ProcessAsync(
        IEnumerable<AdCandidate> candidates, 
        AdContext context)
    {
        // ʵ��ҵ���߼�
        return new ProcessingResult { Success = true };
    }

    public async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return _options.IsEnabled ? HealthStatus.Healthy : HealthStatus.Degraded;
    }
    
    private void OnConfigurationChanged(UserInterestRecallOptions newOptions)
    {
        _logger.LogInformation("Configuration updated for {Strategy}", nameof(UserInterestRecallStrategy));
    }
}

// ����ѡ���� - �Զ��󶨵� "Strategies:UserInterestRecall" ���ý�
public class UserInterestRecallOptions
{
    public int MaxCandidates { get; set; } = 1000;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(15);
    public double ScoreThreshold { get; set; } = 0.3;
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; } = 100;
}
```

### 3. �����ṩ�����

```csharp
// �����ṩ��ʾ��
public class UserProfileProvider : IDataAccessProvider, IHealthCheckable
{
    private readonly UserProfileProviderOptions _options;
    
    public UserProfileProvider(IOptionsMonitor<UserProfileProviderOptions> options)
    {
        _options = options.CurrentValue;
        options.OnChange(OnConfigurationChanged);
    }

    public async Task<UserProfile> GetUserProfileAsync(string userId)
    {
        // ʹ������ֵʵ��ҵ���߼�
        using var connection = new SqlConnection(_options.ConnectionString);
        // ...
    }

    public async Task<HealthStatus> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.IsEnabled)
            return HealthStatus.Degraded;
            
        // ������ݿ�����
        try
        {
            using var connection = new SqlConnection(_options.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return HealthStatus.Healthy;
        }
        catch
        {
            return HealthStatus.Unhealthy;
        }
    }
    
    private void OnConfigurationChanged(UserProfileProviderOptions newOptions)
    {
        // �������ñ��
    }
}

// ����ѡ�� - �Զ��󶨵� "DataProviders:UserProfile" ���ý�
public class UserProfileProviderOptions
{
    public string ConnectionString { get; set; } = "";
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
    public bool IsEnabled { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
}
```

## Լ������

### �������Լ��

| �������   | ������ʽ           | ��������  | ����·��                 |
| ---------- | ------------------ | --------- | ------------------------ |
| �������   | `*Strategy`        | Transient | `Strategies:{����}`      |
| �������   | `*Service`         | Singleton | `Services:{����}`        |
| ��������� | `*Manager`         | Singleton | `Managers:{����}`        |
| �����ṩ�� | `*Provider`        | Singleton | `DataProviders:{����Դ}` |
| ����ƥ���� | `*Matcher`         | Scoped    | `Matchers:{����}`        |
| ������     | `*Calculator`      | Scoped    | `Calculators:{����}`     |
| ������     | `*Processor`       | Scoped    | `Processors:{����}`      |

### ������Լ��

- ������������`{ģ����}Options`
- �Զ�ӳ�����
  - `AdEngineOptions` �� `"AdEngine"` ���ý�
  - `UserInterestRecallOptions` �� `"Strategies:UserInterestRecall"` ���ý�

### �ӿ�Լ��

- ������飺ʵ�� `IHealthCheckable` �ӿ��Զ���ӽ������
- �����������ʵ�� `IConfigurable` �ӿ�֧���Զ������ø����߼�

## �߼�����

### ��ʽ������

```csharp
[Component(Priority = 100, IsEnabled = true, Description = "�߼��û���Ȥ����")]
[ConfigurationBinding("CustomPath:AdvancedStrategy")]
[Singleton]
public class AdvancedUserInterestStrategy : IAdProcessingStrategy
{
    // ���ʵ��
}
```

### ����ѡ��ģʽ

��������������ͬ������ѡ����ʱ��

```csharp
public class RTBAdEngineService : IAdEngineService
{
    public RTBAdEngineService(IOptionsSnapshot<AdEngineOptions> optionsSnapshot)
    {
        // ʹ���������� "RTB"
        var options = optionsSnapshot.Get("RTB");
    }
}

public class DirectSaleAdEngineService : IAdEngineService
{
    public DirectSaleAdEngineService(IOptionsSnapshot<AdEngineOptions> optionsSnapshot)
    {
        // ʹ���������� "DirectSale"
        var options = optionsSnapshot.Get("DirectSale");
    }
}
```

�����ļ���
```json
{
  "AdEngine": {
    "RTB": {
      "DefaultTimeout": "00:00:30",
      "MaxRetries": 3
    },
    "DirectSale": {
      "DefaultTimeout": "00:01:00", 
      "MaxRetries": 5
    }
  }
}
```

### ������֤

```csharp
public class UserInterestRecallOptionsValidator : IValidateOptions<UserInterestRecallOptions>
{
    public ValidateOptionsResult Validate(string name, UserInterestRecallOptions options)
    {
        var failures = new List<string>();

        if (options.MaxCandidates <= 0)
            failures.Add("MaxCandidates must be greater than 0");

        if (options.ScoreThreshold < 0 || options.ScoreThreshold > 1)
            failures.Add("ScoreThreshold must be between 0 and 1");

        return failures.Any() 
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
```

## �����ļ�ģ��

��ο� `appsettings.example.json` �ļ����˽������������ļ��ṹ��

## ���ʵ��

1. **��ѭԼ��**��ʹ�ñ�׼������Լ�������������õı���
2. **����������**��ʹ�� `IOptionsMonitor<T>` ���� `IOptions<T>`
3. **�������**��Ϊ�ؼ����ʵ�� `IHealthCheckable` �ӿ�
4. **������֤**��Ϊ��������ʵ�� `IValidateOptions<T>` ��֤��
5. **������**�����ʵ����Ҫ���ʵ����쳣����
6. **��־��¼**��ʹ�� `ILogger<T>` ��¼�ؼ������ʹ���

## �����ų�

### ���δ���Զ�ע��

1. �����������Ƿ����Լ������ `*Strategy`��`*Service` �ȣ�
2. ȷ��������ǹ��������ࣨ�ǳ��󡢷Ƿ��Ͷ��壩
3. �������Ƿ���ɨ�跶Χ��

### ����δ��Ч

1. ��������������Ƿ����Լ������ `*Options`��
2. ȷ�������ļ�·����ȷ
3. ��֤ JSON ��ʽ��ȷ��

### ������鲻����

1. ȷ�����ʵ���� `IHealthCheckable` �ӿ�
2. ����Ƿ������ `AddComponentHealthChecks()`
3. ��֤�������˵�����

���������ʩ������򻯹��ϵͳ�Ŀ�����ά���������ÿ�����רע��ҵ���߼�ʵ�֡�