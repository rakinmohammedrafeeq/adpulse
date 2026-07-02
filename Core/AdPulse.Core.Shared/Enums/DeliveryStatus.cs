namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// Ͷ��״̬ö��
/// </summary>
public enum DeliveryStatus
{
    /// <summary>
    /// ��Ͷ��
    /// </summary>
    Pending = 1,

    /// <summary>
    /// ��Ͷ��
    /// </summary>
    Delivered = 2,

    /// <summary>
    /// Ͷ�ųɹ�
    /// </summary>
    Success = 3,

    /// <summary>
    /// Ͷ��ʧ��
    /// </summary>
    Failed = 4,

    /// <summary>
    /// ��ȡ��
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// �ѹ���
    /// </summary>
    Expired = 6,

    /// <summary>
    /// ��ʱ
    /// </summary>
    Timeout = 7
}