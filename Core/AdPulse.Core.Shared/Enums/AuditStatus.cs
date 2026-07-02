namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// ���״̬ö��
/// </summary>
public enum AuditStatus
{
    /// <summary>
    /// �����
    /// </summary>
    Pending = 1,

    /// <summary>
    /// �����
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// ���ͨ��
    /// </summary>
    Approved = 3,

    /// <summary>
    /// ��˾ܾ�
    /// </summary>
    Rejected = 4,

    /// <summary>
    /// ��Ҫ�޸�
    /// </summary>
    RequiresChanges = 5,

    /// <summary>
    /// �ѹ���
    /// </summary>
    Expired = 6
}