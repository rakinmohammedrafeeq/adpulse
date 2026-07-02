using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ������������ֵ����
/// </summary>
public class ProfileQualityScore : ValueObject
{
    /// <summary>
    /// ����������
    /// </summary>
    public int CompletenessScore { get; private set; }

    /// <summary>
    /// ׼ȷ������
    /// </summary>
    public int AccuracyScore { get; private set; }

    /// <summary>
    /// ʱЧ������
    /// </summary>
    public int FreshnessScore { get; private set; }

    /// <summary>
    /// �ۺ�����
    /// </summary>
    public int OverallScore { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private ProfileQualityScore() { }

    /// <summary>
    /// ���캯��
    /// </summary>
    public ProfileQualityScore(
        int completenessScore,
        int accuracyScore,
        int freshnessScore)
    {
        CompletenessScore = Math.Max(0, Math.Min(100, completenessScore));
        AccuracyScore = Math.Max(0, Math.Min(100, accuracyScore));
        FreshnessScore = Math.Max(0, Math.Min(100, freshnessScore));
        
        // �����ۺ����֣���Ȩƽ����
        OverallScore = (int)Math.Round(
            (CompletenessScore * 0.4 + 
             AccuracyScore * 0.4 + 
             FreshnessScore * 0.2));
    }

    /// <summary>
    /// ����Ĭ����������
    /// </summary>
    public static ProfileQualityScore CreateDefault()
    {
        return new ProfileQualityScore(0, 100, 100);
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CompletenessScore;
        yield return AccuracyScore;
        yield return FreshnessScore;
        yield return OverallScore;
    }
}