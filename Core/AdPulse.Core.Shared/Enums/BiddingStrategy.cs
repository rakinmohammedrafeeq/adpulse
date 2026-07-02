namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// ���۲���ö��
/// </summary>
public enum BiddingStrategy
{
    /// <summary>
    /// �̶�����
    /// </summary>
    FixedBid = 1,

    /// <summary>
    /// �Զ�����
    /// </summary>
    AutoBid = 2,

    /// <summary>
    /// Ŀ��CPA��ÿ�λ�ͳɱ���
    /// </summary>
    TargetCPA = 3,

    /// <summary>
    /// Ŀ��ROAS�����֧���ر��ʣ�
    /// </summary>
    TargetROAS = 4,

    /// <summary>
    /// ��󻯵��
    /// </summary>
    MaximizeClicks = 5,

    /// <summary>
    /// ���ת��
    /// </summary>
    MaximizeConversions = 6,

    /// <summary>
    /// ���ת����ֵ
    /// </summary>
    MaximizeConversionValue = 7
}