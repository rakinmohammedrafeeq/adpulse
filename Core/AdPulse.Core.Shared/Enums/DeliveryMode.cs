namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// Ͷ��ģʽö��
/// </summary>
public enum DeliveryMode
{
    /// <summary>
    /// ����Ͷ�ţ�Guaranteed��
    /// </summary>
    Guaranteed = 1,

    /// <summary>
    /// ���ȼ�Ͷ�ţ�Priority��
    /// </summary>
    Priority = 2,

    /// <summary>
    /// ����Ͷ�ţ�Bidding/RTB��
    /// </summary>
    Bidding = 3,

    /// <summary>
    /// ���Ͷ�ţ�Mixed��
    /// </summary>
    Mixed = 4
}