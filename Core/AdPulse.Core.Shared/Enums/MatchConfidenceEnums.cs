namespace AdPulse.Core.Shared.Enums;

/// <summary>
/// ���Ŷȵȼ�ö��
/// </summary>
public enum ConfidenceLevel
{
    /// <summary>
    /// �������Ŷȣ�0-0.2��
    /// </summary>
    VeryLow = 1,
    
    /// <summary>
    /// �����Ŷȣ�0.2-0.4��
    /// </summary>
    Low = 2,
    
    /// <summary>
    /// �е����Ŷȣ�0.4-0.6��
    /// </summary>
    Medium = 3,
    
    /// <summary>
    /// �����Ŷȣ�0.6-0.8��
    /// </summary>
    High = 4,
    
    /// <summary>
    /// �������Ŷȣ�0.8-1.0��
    /// </summary>
    VeryHigh = 5
}

/// <summary>
/// �ɿ�������ö��
/// </summary>
public enum ReliabilityRating
{
    /// <summary>
    /// ���ɿ�
    /// </summary>
    Unreliable = 1,
    
    /// <summary>
    /// ���޿ɿ�
    /// </summary>
    LimitedReliability = 2,
    
    /// <summary>
    /// �����ɿ�
    /// </summary>
    BasicReliability = 3,
    
    /// <summary>
    /// �߶ȿɿ�
    /// </summary>
    HighReliability = 4,
    
    /// <summary>
    /// ��ȫ�ɿ�
    /// </summary>
    FullReliability = 5
}