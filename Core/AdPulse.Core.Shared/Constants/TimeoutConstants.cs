namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// ��ʱ���ó���
/// </summary>
public static class TimeoutConstants
{
    /// <summary>
    /// ����������ʱʱ�� (����)
    /// </summary>
    public const int AdRequestTimeout = 100;

    /// <summary>
    /// ��������ʱʱ�� (����)
    /// </summary>
    public const int BidRequestTimeout = 80;

    /// <summary>
    /// ���ݿ��ѯ��ʱʱ�� (��)
    /// </summary>
    public const int DatabaseQueryTimeout = 30;

    /// <summary>
    /// ������ʳ�ʱʱ�� (����)
    /// </summary>
    public const int CacheAccessTimeout = 50;

    /// <summary>
    /// HTTP�ͻ��˳�ʱʱ�� (��)
    /// </summary>
    public const int HttpClientTimeout = 10;

    /// <summary>
    /// �ⲿ������ó�ʱʱ�� (����)
    /// </summary>
    public const int ExternalServiceTimeout = 200;

    /// <summary>
    /// �û��Ự��ʱʱ�� (����)
    /// </summary>
    public const int UserSessionTimeout = 30;

    /// <summary>
    /// ����Ĭ�Ϲ���ʱ�� (����)
    /// </summary>
    public const int DefaultCacheExpiration = 60;
}