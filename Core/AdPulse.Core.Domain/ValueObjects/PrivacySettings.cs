using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ��˽����ֵ����
/// </summary>
public class PrivacySettings : ValueObject
{
    /// <summary>
    /// �Ƿ�ͬ�������ռ�
    /// </summary>
    public bool ConsentToDataCollection { get; private set; } = false;

    /// <summary>
    /// �Ƿ�ͬ�����ݴ���
    /// </summary>
    public bool ConsentToDataProcessing { get; private set; } = false;

    /// <summary>
    /// �Ƿ�ͬ�����ݹ���
    /// </summary>
    public bool ConsentToDataSharing { get; private set; } = false;

    /// <summary>
    /// �Ƿ�ͬ��Ӫ����;
    /// </summary>
    public bool ConsentToMarketing { get; private set; } = false;

    /// <summary>
    /// ͬ��ʱ��
    /// </summary>
    public DateTime? ConsentTimestamp { get; private set; }

    /// <summary>
    /// ͬ��汾
    /// </summary>
    public string? ConsentVersion { get; private set; }

    /// <summary>
    /// ͬ��IP��ַ
    /// </summary>
    public string? ConsentIpAddress { get; private set; }

    /// <summary>
    /// ͬ���û�����
    /// </summary>
    public string? ConsentUserAgent { get; private set; }

    /// <summary>
    /// ���ݱ������ޣ�������
    /// </summary>
    public int? DataRetentionDays { get; private set; }

    /// <summary>
    /// �Ƿ�����ɾ������
    /// </summary>
    public bool RequestDataDeletion { get; private set; } = false;

    /// <summary>
    /// ����ɾ������ʱ��
    /// </summary>
    public DateTime? DataDeletionRequestedAt { get; private set; }

    /// <summary>
    /// �Ƿ�ѡ���˳�
    /// </summary>
    public bool OptOut { get; private set; } = false;

    /// <summary>
    /// ѡ���˳�ʱ��
    /// </summary>
    public DateTime? OptOutTimestamp { get; private set; }

    /// <summary>
    /// GDPR�������
    /// </summary>
    public GdprSettings? GdprSettings { get; private set; }

    /// <summary>
    /// CCPA�������
    /// </summary>
    public CcpaSettings? CcpaSettings { get; private set; }

    /// <summary>
    /// �Զ�����˽����
    /// </summary>
    public IReadOnlyList<ContextProperty> CustomSettings { get; private set; } = new List<ContextProperty>();

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private PrivacySettings() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public PrivacySettings(
        bool consentToDataCollection = false,
        bool consentToDataProcessing = false,
        bool consentToDataSharing = false,
        bool consentToMarketing = false,
        DateTime? consentTimestamp = null,
        string? consentVersion = null,
        string? consentIpAddress = null,
        string? consentUserAgent = null,
        int? dataRetentionDays = null,
        bool requestDataDeletion = false,
        DateTime? dataDeletionRequestedAt = null,
        bool optOut = false,
        DateTime? optOutTimestamp = null,
        GdprSettings? gdprSettings = null,
        CcpaSettings? ccpaSettings = null,
        IEnumerable<ContextProperty>? customSettings = null)
    {
        ValidateDataRetentionDays(dataRetentionDays);

        ConsentToDataCollection = consentToDataCollection;
        ConsentToDataProcessing = consentToDataProcessing;
        ConsentToDataSharing = consentToDataSharing;
        ConsentToMarketing = consentToMarketing;
        ConsentTimestamp = consentTimestamp;
        ConsentVersion = consentVersion;
        ConsentIpAddress = consentIpAddress;
        ConsentUserAgent = consentUserAgent;
        DataRetentionDays = dataRetentionDays;
        RequestDataDeletion = requestDataDeletion;
        DataDeletionRequestedAt = dataDeletionRequestedAt;
        OptOut = optOut;
        OptOutTimestamp = optOutTimestamp;
        GdprSettings = gdprSettings;
        CcpaSettings = ccpaSettings;
        CustomSettings = customSettings?.ToList() ?? new List<ContextProperty>();
    }

    /// <summary>
    /// ����Ĭ����˽����
    /// </summary>
    public static PrivacySettings CreateDefault()
    {
        return new PrivacySettings();
    }

    /// <summary>
    /// ���ֵ䴴����˽���ã������ݣ�
    /// </summary>
    public static PrivacySettings CreateFromDictionary(
        bool consentToDataCollection = false,
        bool consentToDataProcessing = false,
        bool consentToDataSharing = false,
        bool consentToMarketing = false,
        DateTime? consentTimestamp = null,
        string? consentVersion = null,
        string? consentIpAddress = null,
        string? consentUserAgent = null,
        int? dataRetentionDays = null,
        bool requestDataDeletion = false,
        DateTime? dataDeletionRequestedAt = null,
        bool optOut = false,
        DateTime? optOutTimestamp = null,
        GdprSettings? gdprSettings = null,
        CcpaSettings? ccpaSettings = null,
        IDictionary<string, object>? customSettings = null)
    {
        var contextProperties = customSettings?.Select(kvp =>
            new ContextProperty(
                kvp.Key,
                kvp.Value?.ToString() ?? string.Empty,
                kvp.Value?.GetType().Name ?? "String",
                "Privacy",
                false,
                1.0m,
                null,
                "PrivacySettings")
        ).ToList() ?? new List<ContextProperty>();

        return new PrivacySettings(
            consentToDataCollection,
            consentToDataProcessing,
            consentToDataSharing,
            consentToMarketing,
            consentTimestamp,
            consentVersion,
            consentIpAddress,
            consentUserAgent,
            dataRetentionDays,
            requestDataDeletion,
            dataDeletionRequestedAt,
            optOut,
            optOutTimestamp,
            gdprSettings,
            ccpaSettings,
            contextProperties);
    }

    /// <summary>
    /// ����ȫ��ͬ�����˽����
    /// </summary>
    public static PrivacySettings CreateFullConsent(
        string consentVersion,
        string ipAddress,
        string userAgent)
    {
        return new PrivacySettings(
            consentToDataCollection: true,
            consentToDataProcessing: true,
            consentToDataSharing: true,
            consentToMarketing: true,
            consentTimestamp: DateTime.UtcNow,
            consentVersion: consentVersion,
            consentIpAddress: ipAddress,
            consentUserAgent: userAgent);
    }

    /// <summary>
    /// ������Сͬ�����˽����
    /// </summary>
    public static PrivacySettings CreateMinimalConsent(
        string consentVersion,
        string ipAddress,
        string userAgent)
    {
        return new PrivacySettings(
            consentToDataCollection: true,
            consentToDataProcessing: true,
            consentToDataSharing: false,
            consentToMarketing: false,
            consentTimestamp: DateTime.UtcNow,
            consentVersion: consentVersion,
            consentIpAddress: ipAddress,
            consentUserAgent: userAgent);
    }

    /// <summary>
    /// �Ƿ���Чͬ��
    /// </summary>
    public bool HasValidConsent()
    {
        return ConsentTimestamp.HasValue &&
               !string.IsNullOrWhiteSpace(ConsentVersion) &&
               (ConsentToDataCollection || ConsentToDataProcessing);
    }

    /// <summary>
    /// �Ƿ��������Ӫ��
    /// </summary>
    public bool CanUseForMarketing()
    {
        return HasValidConsent() &&
               ConsentToMarketing &&
               !OptOut &&
               !RequestDataDeletion;
    }

    /// <summary>
    /// �Ƿ���Թ�������
    /// </summary>
    public bool CanShareData()
    {
        return HasValidConsent() &&
               ConsentToDataSharing &&
               !OptOut &&
               !RequestDataDeletion;
    }

    /// <summary>
    /// �Ƿ���Ҫɾ������
    /// </summary>
    public bool ShouldDeleteData()
    {
        if (RequestDataDeletion)
            return true;

        if (DataRetentionDays.HasValue && ConsentTimestamp.HasValue)
        {
            var retentionExpiry = ConsentTimestamp.Value.AddDays(DataRetentionDays.Value);
            return DateTime.UtcNow > retentionExpiry;
        }

        return false;
    }

    /// <summary>
    /// ��ȡ�Զ�������ֵ
    /// </summary>
    public T? GetCustomSetting<T>(string key)
    {
        var contextProperty = CustomSettings.FirstOrDefault(p => p.PropertyKey == key);
        if (contextProperty != null)
        {
            return contextProperty.GetValue<T>();
        }

        return default;
    }

    /// <summary>
    /// ��֤���ݱ�������
    /// </summary>
    private static void ValidateDataRetentionDays(int? dataRetentionDays)
    {
        if (dataRetentionDays.HasValue && (dataRetentionDays.Value < 1 || dataRetentionDays.Value > 3650))
            throw new ArgumentOutOfRangeException(nameof(dataRetentionDays), "���ݱ�������������1-3650��֮��");
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ConsentToDataCollection;
        yield return ConsentToDataProcessing;
        yield return ConsentToDataSharing;
        yield return ConsentToMarketing;
        yield return ConsentTimestamp ?? DateTime.MinValue;
        yield return ConsentVersion ?? string.Empty;
        yield return ConsentIpAddress ?? string.Empty;
        yield return ConsentUserAgent ?? string.Empty;
        yield return DataRetentionDays ?? 0;
        yield return RequestDataDeletion;
        yield return DataDeletionRequestedAt ?? DateTime.MinValue;
        yield return OptOut;
        yield return OptOutTimestamp ?? DateTime.MinValue;
        yield return GdprSettings ?? new object();
        yield return CcpaSettings ?? new object();
    }
}

/// <summary>
/// GDPR����
/// </summary>
public class GdprSettings : ValueObject
{
    /// <summary>
    /// �Ƿ�����GDPR
    /// </summary>
    public bool IsApplicable { get; private set; } = false;

    /// <summary>
    /// ͬ�⳷��ʱ��
    /// </summary>
    public DateTime? ConsentWithdrawnAt { get; private set; }

    /// <summary>
    /// ���ݿ�ЯȨ����
    /// </summary>
    public bool RequestDataPortability { get; private set; } = false;

    /// <summary>
    /// ���ݸ���Ȩ����
    /// </summary>
    public bool RequestDataRectification { get; private set; } = false;

    /// <summary>
    /// ��������Ȩ����
    /// </summary>
    public bool RequestProcessingRestriction { get; private set; } = false;

    public GdprSettings(
        bool isApplicable = false,
        DateTime? consentWithdrawnAt = null,
        bool requestDataPortability = false,
        bool requestDataRectification = false,
        bool requestProcessingRestriction = false)
    {
        IsApplicable = isApplicable;
        ConsentWithdrawnAt = consentWithdrawnAt;
        RequestDataPortability = requestDataPortability;
        RequestDataRectification = requestDataRectification;
        RequestProcessingRestriction = requestProcessingRestriction;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsApplicable;
        yield return ConsentWithdrawnAt ?? DateTime.MinValue;
        yield return RequestDataPortability;
        yield return RequestDataRectification;
        yield return RequestProcessingRestriction;
    }
}

/// <summary>
/// CCPA����
/// </summary>
public class CcpaSettings : ValueObject
{
    /// <summary>
    /// �Ƿ�����CCPA
    /// </summary>
    public bool IsApplicable { get; private set; } = false;

    /// <summary>
    /// ѡ���˳����۸�����Ϣ
    /// </summary>
    public bool OptOutOfSale { get; private set; } = false;

    /// <summary>
    /// ѡ���˳�ʱ��
    /// </summary>
    public DateTime? OptOutOfSaleTimestamp { get; private set; }

    /// <summary>
    /// ����ɾ��������Ϣ
    /// </summary>
    public bool RequestPersonalInfoDeletion { get; private set; } = false;

    public CcpaSettings(
        bool isApplicable = false,
        bool optOutOfSale = false,
        DateTime? optOutOfSaleTimestamp = null,
        bool requestPersonalInfoDeletion = false)
    {
        IsApplicable = isApplicable;
        OptOutOfSale = optOutOfSale;
        OptOutOfSaleTimestamp = optOutOfSaleTimestamp;
        RequestPersonalInfoDeletion = requestPersonalInfoDeletion;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IsApplicable;
        yield return OptOutOfSale;
        yield return OptOutOfSaleTimestamp ?? DateTime.MinValue;
        yield return RequestPersonalInfoDeletion;
    }
}