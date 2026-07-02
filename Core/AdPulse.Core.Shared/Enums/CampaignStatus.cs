namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// �״̬ö��
/// </summary>
public enum CampaignStatus
{
    /// <summary>
    /// �ݸ�״̬
    /// </summary>
    Draft = 1,

    /// <summary>
    /// �Ѽƻ�
    /// </summary>
    Scheduled = 2,

    /// <summary>
    /// ������
    /// </summary>
    Running = 3,

    /// <summary>
    /// ��Ծ
    /// </summary>
    Active = 9,

    /// <summary>
    /// ��ͣ
    /// </summary>
    Paused = 4,

    /// <summary>
    /// �����
    /// </summary>
    Completed = 5,

    /// <summary>
    /// ��ȡ��
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Ԥ��ľ�
    /// </summary>
    BudgetExhausted = 7,

    /// <summary>
    /// �ѹ���
    /// </summary>
    Expired = 8
}