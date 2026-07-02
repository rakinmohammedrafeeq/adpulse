namespace AdPulse.Core.Shared.Helpers;

/// <summary>
/// ��������ɰ�����
/// </summary>
public static class CacheKeyHelper
{
    /// <summary>
    /// ������ָ��
    /// </summary>
    private const string Separator = ":";

    /// <summary>
    /// �����ǰ׺
    /// </summary>
    public static class Prefixes
    {
        /// <summary>
        /// �û���������ǰ׺
        /// </summary>
        public const string UserFeature = "user";

        /// <summary>
        /// ��������������ǰ׺
        /// </summary>
        public const string ContextFeature = "context";

        /// <summary>
        /// �����������ǰ׺
        /// </summary>
        public const string AdFeature = "ad";

        /// <summary>
        /// ��˽������ǰ׺
        /// </summary>
        public const string AuditResult = "audit";

        /// <summary>
        /// �Ự����ǰ׺
        /// </summary>
        public const string Session = "session";

        /// <summary>
        /// ���û���ǰ׺
        /// </summary>
        public const string Configuration = "config";

        /// <summary>
        /// ָ�껺��ǰ׺
        /// </summary>
        public const string Metrics = "metrics";

        /// <summary>
        /// ������ǰ׺
        /// </summary>
        public const string Lock = "lock";
    }

    /// <summary>
    /// �����û����������
    /// </summary>
    /// <param name="userId">�û�ID</param>
    /// <param name="featureType">��������</param>
    /// <returns>�����</returns>
    public static string GenerateUserFeatureKey(string userId, string featureType = "feature")
    {
        return BuildKey(Prefixes.UserFeature, userId, featureType);
    }

    /// <summary>
    /// �������������������
    /// </summary>
    /// <param name="contextId">������ID</param>
    /// <param name="featureType">��������</param>
    /// <returns>�����</returns>
    public static string GenerateContextFeatureKey(string contextId, string featureType = "feature")
    {
        return BuildKey(Prefixes.ContextFeature, contextId, featureType);
    }

    /// <summary>
    /// ���ɹ�����������
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <param name="featureType">��������</param>
    /// <returns>�����</returns>
    public static string GenerateAdFeatureKey(string adId, string featureType = "feature")
    {
        return BuildKey(Prefixes.AdFeature, adId, featureType);
    }

    /// <summary>
    /// ������˽�������
    /// </summary>
    /// <param name="adId">���ID</param>
    /// <returns>�����</returns>
    public static string GenerateAuditResultKey(string adId)
    {
        return BuildKey(Prefixes.AuditResult, adId, "result");
    }

    /// <summary>
    /// ���ɻỰ�����
    /// </summary>
    /// <param name="sessionId">�ỰID</param>
    /// <returns>�����</returns>
    public static string GenerateSessionKey(string sessionId)
    {
        return BuildKey(Prefixes.Session, sessionId);
    }

    /// <summary>
    /// �������û����
    /// </summary>
    /// <param name="configKey">���ü�</param>
    /// <returns>�����</returns>
    public static string GenerateConfigurationKey(string configKey)
    {
        return BuildKey(Prefixes.Configuration, configKey);
    }

    /// <summary>
    /// ����ָ�껺���
    /// </summary>
    /// <param name="metricType">ָ������</param>
    /// <param name="metricId">ָ��ID</param>
    /// <returns>�����</returns>
    public static string GenerateMetricsKey(string metricType, string metricId)
    {
        return BuildKey(Prefixes.Metrics, metricType, metricId);
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="lockType">������</param>
    /// <param name="lockId">��ID</param>
    /// <returns>�����</returns>
    public static string GenerateLockKey(string lockType, string lockId)
    {
        return BuildKey(Prefixes.Lock, lockType, lockId);
    }

    /// <summary>
    /// ���ɴ�ʱ����Ļ����
    /// </summary>
    /// <param name="baseKey">������</param>
    /// <param name="timestamp">ʱ���</param>
    /// <returns>�����</returns>
    public static string GenerateTimestampKey(string baseKey, long timestamp)
    {
        return BuildKey(baseKey, timestamp.ToString());
    }

    /// <summary>
    /// ���ɴ����ڵĻ����
    /// </summary>
    /// <param name="baseKey">������</param>
    /// <param name="date">����</param>
    /// <returns>�����</returns>
    public static string GenerateDateKey(string baseKey, DateTime date)
    {
        return BuildKey(baseKey, date.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="parts">���ĸ�������</param>
    /// <returns>�����Ļ����</returns>
    private static string BuildKey(params string[] parts)
    {
        var validParts = parts.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(Separator, validParts);
    }

    /// <summary>
    /// ���������
    /// </summary>
    /// <param name="key">�����</param>
    /// <returns>���ĸ�������</returns>
    public static string[] ParseKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return Array.Empty<string>();

        return key.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// ��ȡ�������ǰ׺
    /// </summary>
    /// <param name="key">�����</param>
    /// <returns>ǰ׺</returns>
    public static string GetPrefix(string key)
    {
        var parts = ParseKey(key);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    /// <summary>
    /// ����ͨ���ģʽ
    /// </summary>
    /// <param name="prefix">ǰ׺</param>
    /// <param name="pattern">ģʽ</param>
    /// <returns>ͨ���ģʽ</returns>
    public static string BuildWildcardPattern(string prefix, string pattern = "*")
    {
        return BuildKey(prefix, pattern);
    }
}