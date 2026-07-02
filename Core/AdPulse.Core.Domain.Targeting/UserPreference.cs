using AdPulse.Core.Domain.ValueObjects;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Targeting;

/// <summary>
/// �û�ƫ�ö���������
/// �̳���TargetingContextBase���ṩ�û�ƫ�����ݵĶ��������Ĺ���
/// רע�ڹ��Ͷ����ص��û�ƫ�����ú���˽����
/// </summary>
public class UserPreference : TargetingContextBase
{
    /// <summary>
    /// ����������
    /// </summary>
    public override string ContextName => "�û�ƫ��������";

    /// <summary>
    /// ÿ�չ���ع��������
    /// </summary>
    public int? MaxDailyAdImpressions => GetPropertyValue<int?>("MaxDailyAdImpressions");

    /// <summary>
    /// ƫ�õĹ������
    /// </summary>
    public IReadOnlyList<AdType> PreferredAdTypes => GetPropertyValue<List<AdType>>("PreferredAdTypes") ?? new List<AdType>();

    /// <summary>
    /// ���εĹ�����
    /// </summary>
    public IReadOnlyList<string> BlockedCategories => GetPropertyValue<List<string>>("BlockedCategories") ?? new List<string>();

    /// <summary>
    /// ƫ�õ���������
    /// </summary>
    public IReadOnlyList<string> PreferredTopics => GetPropertyValue<List<string>>("PreferredTopics") ?? new List<string>();

    /// <summary>
    /// �Ƿ�������Ի����
    /// </summary>
    public bool AllowPersonalizedAds => GetPropertyValue("AllowPersonalizedAds", true);

    /// <summary>
    /// �Ƿ�������Ϊ׷��
    /// </summary>
    public bool AllowBehaviorTracking => GetPropertyValue("AllowBehaviorTracking", true);

    /// <summary>
    /// �Ƿ�������豸׷��
    /// </summary>
    public bool AllowCrossDeviceTracking => GetPropertyValue("AllowCrossDeviceTracking", true);

    /// <summary>
    /// ƫ�õĹ��Ͷ��ʱ��
    /// </summary>
    public IReadOnlyList<TimeWindow> PreferredTimeSlots => GetPropertyValue<List<TimeWindow>>("PreferredTimeSlots") ?? new List<TimeWindow>();

    /// <summary>
    /// ��˽����
    /// </summary>
    public PrivacyLevel PrivacyLevel => GetPropertyValue("PrivacyLevel", PrivacyLevel.Standard);

    /// <summary>
    /// ƫ�õ������б�
    /// </summary>
    public IReadOnlyList<string> PreferredLanguages => GetPropertyValue<List<string>>("PreferredLanguages") ?? new List<string>();

    /// <summary>
    /// ���ݳ���ȼ���ƫ��
    /// </summary>
    public ContentMaturityLevel ContentMaturityLevel => GetPropertyValue("ContentMaturityLevel", ContentMaturityLevel.General);

    /// <summary>
    /// �Զ���ƫ������
    /// </summary>
    public IReadOnlyDictionary<string, object> CustomPreferences => GetPropertyValue<Dictionary<string, object>>("CustomPreferences") ?? new Dictionary<string, object>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private UserPreference() : base("UserPreference") { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public UserPreference(
        int? maxDailyAdImpressions = null,
        IList<AdType>? preferredAdTypes = null,
        IList<string>? blockedCategories = null,
        IList<string>? preferredTopics = null,
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        bool allowCrossDeviceTracking = true,
        IList<TimeWindow>? preferredTimeSlots = null,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard,
        IList<string>? preferredLanguages = null,
        ContentMaturityLevel contentMaturityLevel = ContentMaturityLevel.General,
        IDictionary<string, object>? customPreferences = null,
        string? dataSource = null)
        : base("UserPreference", CreateProperties(maxDailyAdImpressions, preferredAdTypes, blockedCategories, preferredTopics, allowPersonalizedAds, allowBehaviorTracking, allowCrossDeviceTracking, preferredTimeSlots, privacyLevel, preferredLanguages, contentMaturityLevel, customPreferences), dataSource)
    {
        ValidateMaxDailyImpressions(maxDailyAdImpressions);
    }

    /// <summary>
    /// ���������ֵ�
    /// </summary>
    private static Dictionary<string, object> CreateProperties(
        int? maxDailyAdImpressions,
        IList<AdType>? preferredAdTypes,
        IList<string>? blockedCategories,
        IList<string>? preferredTopics,
        bool allowPersonalizedAds,
        bool allowBehaviorTracking,
        bool allowCrossDeviceTracking,
        IList<TimeWindow>? preferredTimeSlots,
        PrivacyLevel privacyLevel,
        IList<string>? preferredLanguages,
        ContentMaturityLevel contentMaturityLevel,
        IDictionary<string, object>? customPreferences)
    {
        var properties = new Dictionary<string, object>
        {
            ["AllowPersonalizedAds"] = allowPersonalizedAds,
            ["AllowBehaviorTracking"] = allowBehaviorTracking,
            ["AllowCrossDeviceTracking"] = allowCrossDeviceTracking,
            ["PrivacyLevel"] = privacyLevel,
            ["ContentMaturityLevel"] = contentMaturityLevel
        };

        if (maxDailyAdImpressions.HasValue)
            properties["MaxDailyAdImpressions"] = maxDailyAdImpressions.Value;

        if (preferredAdTypes != null && preferredAdTypes.Any())
            properties["PreferredAdTypes"] = preferredAdTypes.ToList();

        if (blockedCategories != null && blockedCategories.Any())
            properties["BlockedCategories"] = blockedCategories.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        if (preferredTopics != null && preferredTopics.Any())
            properties["PreferredTopics"] = preferredTopics.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

        if (preferredTimeSlots != null && preferredTimeSlots.Any())
            properties["PreferredTimeSlots"] = preferredTimeSlots.ToList();

        if (preferredLanguages != null && preferredLanguages.Any())
            properties["PreferredLanguages"] = preferredLanguages.ToList();

        if (customPreferences != null && customPreferences.Any())
            properties["CustomPreferences"] = customPreferences.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return properties;
    }

    /// <summary>
    /// ����Ĭ��ƫ��������
    /// </summary>
    public static UserPreference CreateDefault(string? dataSource = null)
    {
        return new UserPreference(dataSource: dataSource);
    }

    /// <summary>
    /// ��������ƫ��������
    /// </summary>
    public static UserPreference CreateBasic(
        bool allowPersonalizedAds = true,
        bool allowBehaviorTracking = true,
        PrivacyLevel privacyLevel = PrivacyLevel.Standard,
        string? dataSource = null)
    {
        return new UserPreference(
            allowPersonalizedAds: allowPersonalizedAds,
            allowBehaviorTracking: allowBehaviorTracking,
            privacyLevel: privacyLevel,
            dataSource: dataSource);
    }

    /// <summary>
    /// �����ϸ���˽ƫ��������
    /// </summary>
    public static UserPreference CreateStrictPrivacy(string? dataSource = null)
    {
        return new UserPreference(
            allowPersonalizedAds: false,
            allowBehaviorTracking: false,
            allowCrossDeviceTracking: false,
            privacyLevel: PrivacyLevel.High,
            dataSource: dataSource);
    }

    /// <summary>
    /// �Ƿ�ƫ��ָ���������
    /// </summary>
    public bool PrefersAdType(AdType adType)
    {
        return PreferredAdTypes.Contains(adType);
    }

    /// <summary>
    /// �Ƿ�����ָ�����
    /// </summary>
    public bool IsBlockedCategory(string category)
    {
        return BlockedCategories.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ�ƫ��ָ������
    /// </summary>
    public bool PrefersTopicCategory(string topic)
    {
        return PreferredTopics.Contains(topic, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// �Ƿ���ƫ��ʱ����
    /// </summary>
    public bool IsInPreferredTimeSlot(DateTime timestamp)
    {
        if (!PreferredTimeSlots.Any())
            return true; // û������ƫ��ʱ��ʱ��Ĭ������ʱ�䶼����

        return PreferredTimeSlots.Any(slot => slot.ContainsTime(timestamp));
    }

    /// <summary>
    /// ��ȡ�Զ���ƫ��ֵ
    /// </summary>
    public T? GetCustomPreference<T>(string key)
    {
        if (CustomPreferences.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// �Ƿ���������ʹ��
    /// </summary>
    public bool AllowsDataUsage(DataUsageType usageType)
    {
        return usageType switch
        {
            DataUsageType.Personalization => AllowPersonalizedAds,
            DataUsageType.BehaviorTracking => AllowBehaviorTracking,
            DataUsageType.CrossDeviceTracking => AllowCrossDeviceTracking,
            _ => false
        };
    }

    /// <summary>
    /// ��ȡ�������ƫ������
    /// </summary>
    public decimal GetAdTypePreferenceScore(AdType adType)
    {
        if (!PreferredAdTypes.Any())
            return 1.0m; // û��ƫ������ʱĬ�Ͻ�����������

        return PrefersAdType(adType) ? 1.0m : 0.0m;
    }

    /// <summary>
    /// ��ȡ��������ƥ���
    /// </summary>
    public decimal GetTopicMatchScore(IEnumerable<string> targetTopics)
    {
        if (!targetTopics.Any() || !PreferredTopics.Any())
            return 1.0m;

        var matchCount = targetTopics.Count(topic => PrefersTopicCategory(topic));
        return (decimal)matchCount / targetTopics.Count();
    }

    /// <summary>
    /// ��֤ÿ�չ���ع����
    /// </summary>
    private static void ValidateMaxDailyImpressions(int? maxDailyAdImpressions)
    {
        if (maxDailyAdImpressions.HasValue && (maxDailyAdImpressions.Value < 0 || maxDailyAdImpressions.Value > 1000))
            throw new ArgumentOutOfRangeException(nameof(maxDailyAdImpressions), "ÿ��������ع����������0-1000֮��");
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public override string GetDebugInfo()
    {
        var baseInfo = base.GetDebugInfo();
        var preferenceInfo = $"Privacy:{PrivacyLevel} Personalized:{AllowPersonalizedAds} AdTypes:{PreferredAdTypes.Count} Blocked:{BlockedCategories.Count}";
        return $"{baseInfo} | {preferenceInfo}";
    }

    /// <summary>
    /// ��֤�����ĵ���Ч��
    /// </summary>
    public override bool IsValid()
    {
        if (!base.IsValid())
            return false;

        // ��֤ÿ���ع��������
        if (MaxDailyAdImpressions.HasValue && (MaxDailyAdImpressions.Value < 0 || MaxDailyAdImpressions.Value > 1000))
            return false;

        return true;
    }
}

/// <summary>
/// ��˽����ö��
/// </summary>
public enum PrivacyLevel
{
    Low = 1,
    Standard = 2,
    High = 3,
    Maximum = 4
}

/// <summary>
/// ���ݳ���ȼ���ö��
/// </summary>
public enum ContentMaturityLevel
{
    General = 1,
    Teen = 2,
    Mature = 3,
    Adult = 4,
    Restricted = 5
}

/// <summary>
/// ����ʹ������ö��
/// </summary>
public enum DataUsageType
{
    Personalization = 1,
    BehaviorTracking = 2,
    CrossDeviceTracking = 3
}