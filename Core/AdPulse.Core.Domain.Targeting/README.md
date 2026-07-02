# ITargetingCriteria �� ITargetingContext �ӿ�ʹ��ָ��

## ����

���ĵ��������� `AdPulse.Core.Domain` ��Ŀ����ʵ�ֵ� `ITargetingCriteria` �� `ITargetingContext` �ӿڵ�ʹ�÷������������ӿ�Ϊ��涨������ṩ��ͳһ�ĳ���㣬֧�����Ķ����������ú����������ݹ����

## �ӿ����

### ITargetingCriteria �ӿ�

������������ӿڣ����ڶ���ͳһ�Ķ���������ã�

```csharp
public interface ITargetingCriteria
{
    string CriteriaType { get; }
    IReadOnlyDictionary<string, object> Rules { get; }
    decimal Weight { get; }
    bool IsEnabled { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    
    T? GetRule<T>(string ruleKey);
    T GetRule<T>(string ruleKey, T defaultValue);
    bool HasRule(string ruleKey);
    IReadOnlyCollection<string> GetRuleKeys();
    bool IsValid();
    string GetConfigurationSummary();
}
```

### ITargetingContext �ӿ�

���������ĳ���ӿڣ�����ͳһ���������ݷ��ʣ�

```csharp
public interface ITargetingContext
{
    string ContextType { get; }
    IReadOnlyDictionary<string, object> Properties { get; }
    DateTime Timestamp { get; }
    string ContextId { get; }
    string DataSource { get; }
    
    T? GetProperty<T>(string propertyKey);
    T GetProperty<T>(string propertyKey, T defaultValue);
    string GetPropertyAsString(string propertyKey);
    bool HasProperty(string propertyKey);
    IReadOnlyCollection<string> GetPropertyKeys();
    bool IsValid();
    bool IsExpired(TimeSpan maxAge);
    IReadOnlyDictionary<string, object> GetMetadata();
    string GetDebugInfo();
    ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys);
    ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false);
}
```

## ʵ����

### 1. ����������� (GeoTargetingCriteria)

```csharp
// ���������������
var geoLocations = new List<GeoInfo>
{
    GeoInfo.Create("CN", "�й�", "����"),
    GeoInfo.Create("CN", "�й�", "�Ϻ�")
};

var geoCriteria = GeoTargetingCriteria.Create(
    includedLocations: geoLocations,
    mode: GeoTargetingMode.Include,
    maxDistance: 50.0, // 50���ﷶΧ��
    weight: 1.0m,
    isEnabled: true
);

// ʹ�ýӿڷ���
Console.WriteLine($"��������: {geoCriteria.CriteriaType}");
Console.WriteLine($"����ժҪ: {geoCriteria.GetConfigurationSummary()}");
Console.WriteLine($"������: {geoCriteria.GetRule<double?>("MaxDistance")}");
```

### 2. �û����������� (UserTargetingContext)

```csharp
// �����û�����
var basicInfo = UserBasicInfo.CreateBasic("����", Gender.Male, new DateTime(1990, 1, 1));
var userProfile = UserProfile.Create("user123", basicInfo);

// ��������λ����Ϣ
var geoInfo = GeoInfo.Create("CN", "�й�", "����");

// �����豸��Ϣ
var deviceInfo = DeviceInfo.CreateMobile("Android", "Samsung", "Galaxy S21");

// �����û�����������
var userContext = UserTargetingContext.Create(
    userId: "user123",
    userProfile: userProfile,
    geoLocation: geoInfo,
    deviceInfo: deviceInfo,
    requestTime: DateTime.UtcNow,
    dataSource: "UserService"
);

// ʹ�ýӿڷ���
Console.WriteLine($"����������: {userContext.ContextType}");
Console.WriteLine($"�û�����: {userContext.GetUserAge()}");
Console.WriteLine($"�Ƿ��ƶ��豸: {userContext.IsMobileDevice()}");
Console.WriteLine($"λ������: {userContext.GetLocationDescription()}");
```

### 3. �� AdContext ת��

```csharp
// �����е� AdContext �����û�����������
public static UserTargetingContext ConvertFromAdContext(AdContext adContext)
{
    return UserTargetingContext.FromAdContext(adContext);
}

// ʹ��ʾ��
var adContext = AdContext.Create(
    requestId: "req123",
    userId: "user456",
    placementId: "placement789",
    device: deviceInfo,
    geoLocation: geoInfo,
    userAgent: "Mozilla/5.0...",
    requestTime: DateTime.UtcNow,
    timeWindow: timeWindow,
    requestSource: RequestSource.Website,
    adSize: AdSize.CreateStandard(StandardAdSize.Banner)
);

var targetingContext = UserTargetingContext.FromAdContext(adContext);
```

## ��չ�Զ���ʵ��

### �����Զ��嶨������

```csharp
public class InterestTargetingCriteria : TargetingCriteriaBase
{
    public override string CriteriaType => "Interest";
    
    public IReadOnlyList<string> TargetInterests => 
        GetRule<List<string>>("TargetInterests") ?? new List<string>();
    
    public decimal MinMatchThreshold => 
        GetRule<decimal>("MinMatchThreshold", 0.6m);
    
    public InterestTargetingCriteria(
        IList<string> targetInterests,
        decimal minMatchThreshold = 0.6m,
        decimal weight = 1.0m,
        bool isEnabled = true) 
        : base(CreateRules(targetInterests, minMatchThreshold), weight, isEnabled)
    {
    }
    
    private static Dictionary<string, object> CreateRules(
        IList<string> targetInterests,
        decimal minMatchThreshold)
    {
        return new Dictionary<string, object>
        {
            ["TargetInterests"] = targetInterests?.ToList() ?? new List<string>(),
            ["MinMatchThreshold"] = minMatchThreshold
        };
    }
    
    protected override bool ValidateSpecificRules()
    {
        return TargetInterests.Any() && 
               MinMatchThreshold >= 0 && 
               MinMatchThreshold <= 1;
    }
}
```

### �����Զ���������

```csharp
public class BehaviorTargetingContext : TargetingContextBase
{
    public IReadOnlyList<string> RecentActions => 
        GetProperty<List<string>>("RecentActions") ?? new List<string>();
    
    public TimeSpan SessionDuration => 
        GetProperty<TimeSpan>("SessionDuration", TimeSpan.Zero);
    
    public int PageViews => 
        GetProperty<int>("PageViews", 0);
    
    public BehaviorTargetingContext(
        IList<string> recentActions,
        TimeSpan sessionDuration,
        int pageViews,
        string dataSource = "BehaviorService") 
        : base("Behavior", CreateProperties(recentActions, sessionDuration, pageViews), dataSource)
    {
    }
    
    private static Dictionary<string, object> CreateProperties(
        IList<string> recentActions,
        TimeSpan sessionDuration,
        int pageViews)
    {
        return new Dictionary<string, object>
        {
            ["RecentActions"] = recentActions?.ToList() ?? new List<string>(),
            ["SessionDuration"] = sessionDuration,
            ["PageViews"] = pageViews
        };
    }
}
```

## ʹ�����ʵ��

### 1. ���Ͱ�ȫ����

```csharp
// �Ƽ���ʹ�����Ͱ�ȫ�ķ���
var maxDistance = geoCriteria.GetRule<double?>("MaxDistance");
if (maxDistance.HasValue)
{
    // ��������߼�
}

// �Ƽ����ṩĬ��ֵ
var threshold = criteria.GetRule<decimal>("Threshold", 0.5m);
```

### 2. ������֤

```csharp
// ʼ����֤������Ч��
if (!criteria.IsValid())
{
    throw new InvalidOperationException("��������������Ч");
}

if (context.IsExpired(TimeSpan.FromMinutes(30)))
{
    // ˢ�»����»�ȡ����������
}
```

### 3. �����Ż�

```csharp
// ���������������ĸ���
var lightweightContext = userContext.CreateLightweightCopy(new[] 
{
    "UserId", "GeoLocation", "DeviceInfo"
});

// �ϲ����������
var mergedContext = userContext.Merge(behaviorContext, overwriteExisting: false);
```

### 4. ���Ժ���־

```csharp
// ��ȡ������Ϣ
logger.LogDebug("��������: {Summary}", criteria.GetConfigurationSummary());
logger.LogDebug("��������Ϣ: {DebugInfo}", context.GetDebugInfo());

// ��ȡԪ����
var metadata = context.GetMetadata();
foreach (var item in metadata)
{
    logger.LogTrace("Ԫ���� {Key}: {Value}", item.Key, item.Value);
}
```

## ƥ��������

```csharp
// ����ƥ����
var matchResult = TargetingMatchResult.Success(
    score: 0.85m,
    matcherType: "GeoMatcher",
    criteriaType: "Geo",
    executionTime: TimeSpan.FromMilliseconds(15),
    details: new Dictionary<string, object>
    {
        ["MatchedCity"] = "����",
        ["Distance"] = 12.5
    }
);

// ����ƥ����
if (matchResult.IsMatch && matchResult.Score > 0.7m)
{
    Console.WriteLine($"ƥ��ɹ�: {matchResult}");
    var matchedCity = matchResult.GetDetail<string>("MatchedCity");
    Console.WriteLine($"ƥ�����: {matchedCity}");
}
```

## �ܽ�

���׽ӿ�����ṩ�ˣ�

1. **ͳһ�ĳ����**�����ж��������������Ķ�ʵ��ͳһ�ӿ�
2. **���Ͱ�ȫ**��ǿ���͵����ݷ��ʷ���
3. **����չ��**��֧���Զ���ʵ�ָ��ֶ�������
4. **�����Ż�**�������������ĺ͸�Ч�����ݷ���
5. **����֧��**���ḻ�ĵ��Ժ�Ԫ������Ϣ

ͨ����Щ�ӿڣ���涨��ϵͳ����ʵ�ָ�������ά���Ķ�����Թ����