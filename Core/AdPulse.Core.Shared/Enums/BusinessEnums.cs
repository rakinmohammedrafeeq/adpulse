namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// Ԥ��״̬����
/// </summary>
public enum BudgetStatusType
{
    /// <summary>
    /// ��Ծ
    /// </summary>
    Active = 1,

    /// <summary>
    /// �ľ�
    /// </summary>
    Exhausted = 2,

    /// <summary>
    /// ��ͣ
    /// </summary>
    Paused = 3,

    /// <summary>
    /// ����
    /// </summary>
    OverLimit = 4,

    /// <summary>
    /// Ԥ��
    /// </summary>
    Warning = 5
}

/// <summary>
/// ����������
/// </summary>
public enum BlacklistType
{
    /// <summary>
    /// �û�������
    /// </summary>
    User = 1,

    /// <summary>
    /// �豸������
    /// </summary>
    Device = 2,

    /// <summary>
    /// IP������
    /// </summary>
    IpAddress = 3,

    /// <summary>
    /// ��������
    /// </summary>
    Ad = 4,

    /// <summary>
    /// �����������
    /// </summary>
    Advertiser = 5,

    /// <summary>
    /// ����������
    /// </summary>
    Domain = 6,

    /// <summary>
    /// �ؼ��ʺ�����
    /// </summary>
    Keyword = 7,

    /// <summary>
    /// ����λ�ú�����
    /// </summary>
    Geo = 8
}

/// <summary>
/// ���״̬����
/// </summary>
public enum InventoryStatusType
{
    /// <summary>
    /// ����
    /// </summary>
    Available = 1,

    /// <summary>
    /// Ԥ��
    /// </summary>
    Reserved = 2,

    /// <summary>
    /// ����
    /// </summary>
    SoldOut = 3,

    /// <summary>
    /// ά����
    /// </summary>
    Maintenance = 4,

    /// <summary>
    /// ������
    /// </summary>
    Unavailable = 5
}



/// <summary>
/// �����ʽ
/// </summary>
public enum CreativeFormat
{
    /// <summary>
    /// ������
    /// </summary>
    Banner = 1,

    /// <summary>
    /// �������
    /// </summary>
    Interstitial = 2,

    /// <summary>
    /// ��Ƶ���
    /// </summary>
    Video = 3,

    /// <summary>
    /// ԭ�����
    /// </summary>
    Native = 4,

    /// <summary>
    /// �������
    /// </summary>
    Popup = 5,

    /// <summary>
    /// ���ǹ��
    /// </summary>
    Overlay = 6,

    /// <summary>
    /// ����չ���
    /// </summary>
    Expandable = 7,

    /// <summary>
    /// ��ý����
    /// </summary>
    RichMedia = 8
}

/// <summary>
/// ִ��״̬
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// ��ִ��
    /// </summary>
    Pending = 1,

    /// <summary>
    /// ������
    /// </summary>
    Running = 2,

    /// <summary>
    /// �ɹ�
    /// </summary>
    Success = 3,

    /// <summary>
    /// �����
    /// </summary>
    Completed = 4,

    /// <summary>
    /// ��ʧ��
    /// </summary>
    Failed = 5,

    /// <summary>
    /// ��ȡ��
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// ������
    /// </summary>
    Skipped = 7
}



/// <summary>
/// �澯����
/// </summary>
public enum AlertLevel
{
    /// <summary>
    /// ��Ϣ
    /// </summary>
    Info = 1,

    /// <summary>
    /// ����
    /// </summary>
    Warning = 2,

    /// <summary>
    /// ����
    /// </summary>
    Error = 3,

    /// <summary>
    /// ����
    /// </summary>
    Critical = 4,

    /// <summary>
    /// ���
    /// </summary>
    Emergency = 5
}

/// <summary>
/// ��ƽ��
/// </summary>
public enum AuditResult
{
    /// <summary>
    /// �ɹ�
    /// </summary>
    Success = 1,

    /// <summary>
    /// ʧ��
    /// </summary>
    Failure = 2,

    /// <summary>
    /// ���ֳɹ�
    /// </summary>
    PartialSuccess = 3,

    /// <summary>
    /// ����
    /// </summary>
    Skipped = 4,

    /// <summary>
    /// δ��Ȩ
    /// </summary>
    Unauthorized = 5,

    /// <summary>
    /// �ܾ�����
    /// </summary>
    AccessDenied = 6
}