using AdPulse.Core.Domain.Common;
using AdPulse.Core.Domain.Enums;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ��������ֵ����
/// </summary>
public class QualityScore : ValueObject
{
    /// <summary>
    /// ���ID
    /// </summary>
    public Guid AdId { get; private set; }

    /// <summary>
    /// ���������֣�0-10��
    /// </summary>
    public decimal OverallScore { get; private set; }

    /// <summary>
    /// ����Ե÷֣�0-10��
    /// </summary>
    public decimal RelevanceScore { get; private set; }

    /// <summary>
    /// �û�����÷֣�0-10��
    /// </summary>
    public decimal UserExperienceScore { get; private set; }

    /// <summary>
    /// Ԥ�ڵ����
    /// </summary>
    public decimal ExpectedCtr { get; private set; }

    /// <summary>
    /// Ԥ��ת����
    /// </summary>
    public decimal ExpectedCvr { get; private set; }

    /// <summary>
    /// ��ʷ���ֵ÷�
    /// </summary>
    public decimal HistoricalPerformanceScore { get; private set; }

    /// <summary>
    /// ���������÷�
    /// </summary>
    public decimal CreativeQualityScore { get; private set; }

    /// <summary>
    /// ���ҳ�����÷�
    /// </summary>
    public decimal LandingPageQualityScore { get; private set; }

    /// <summary>
    /// ���ּ���ʱ��
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// ���ְ汾
    /// </summary>
    public string? Version { get; private set; }

    /// <summary>
    /// ���Ŷ�
    /// </summary>
    public decimal Confidence { get; private set; }

    /// <summary>
    /// �����ȼ�
    /// </summary>
    public QualityLevel QualityLevel
    {
        get
        {
            return OverallScore switch
            {
                >= 8.5m => QualityLevel.Excellent,
                >= 7.0m => QualityLevel.Good,
                >= 5.0m => QualityLevel.Average,
                >= 3.0m => QualityLevel.Poor,
                _ => QualityLevel.VeryPoor
            };
        }
    }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private QualityScore()
    {
        AdId = Guid.Empty;
        OverallScore = 0;
        RelevanceScore = 0;
        UserExperienceScore = 0;
        ExpectedCtr = 0;
        ExpectedCvr = 0;
        HistoricalPerformanceScore = 0;
        CreativeQualityScore = 0;
        LandingPageQualityScore = 0;
        CalculatedAt = DateTime.UtcNow;
        Confidence = 1.0m;
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public QualityScore(
        Guid adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore,
        string? version = null,
        decimal confidence = 1.0m,
        DateTime? calculatedAt = null)
    {
        ValidateInput(adId, relevanceScore, userExperienceScore, expectedCtr, expectedCvr,
                     historicalPerformanceScore, creativeQualityScore, landingPageQualityScore, confidence);

        AdId = adId;
        RelevanceScore = relevanceScore;
        UserExperienceScore = userExperienceScore;
        ExpectedCtr = expectedCtr;
        ExpectedCvr = expectedCvr;
        HistoricalPerformanceScore = historicalPerformanceScore;
        CreativeQualityScore = creativeQualityScore;
        LandingPageQualityScore = landingPageQualityScore;
        Version = version;
        Confidence = confidence;
        CalculatedAt = calculatedAt ?? DateTime.UtcNow;

        // �������������֣���Ȩƽ����
        OverallScore = CalculateOverallScore();
    }

    /// <summary>
    /// ������������
    /// </summary>
    public static QualityScore Create(
        Guid adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore)
    {
        return new QualityScore(
            adId,
            relevanceScore,
            userExperienceScore,
            expectedCtr,
            expectedCvr,
            historicalPerformanceScore,
            creativeQualityScore,
            landingPageQualityScore);
    }

    /// <summary>
    /// ����Ĭ����������
    /// </summary>
    public static QualityScore CreateDefault(Guid adId)
    {
        return new QualityScore(
            adId,
            5.0m, // �е������
            5.0m, // �е��û�����
            0.02m, // 2% CTR
            0.01m, // 1% CVR
            5.0m, // �е���ʷ����
            5.0m, // �еȴ�������
            5.0m); // �е����ҳ����
    }

    /// <summary>
    /// ��������Ե÷�
    /// </summary>
    public QualityScore UpdateRelevanceScore(decimal newScore)
    {
        return new QualityScore(
            AdId,
            newScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ����Ԥ�ڱ���
    /// </summary>
    public QualityScore UpdateExpectedPerformance(decimal newCtr, decimal newCvr)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            newCtr,
            newCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ������ʷ���ֵ÷�
    /// </summary>
    public QualityScore UpdateHistoricalPerformance(decimal newScore)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            newScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            Confidence,
            DateTime.UtcNow);
    }

    /// <summary>
    /// ���ð汾
    /// </summary>
    public QualityScore WithVersion(string version)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            version,
            Confidence,
            CalculatedAt);
    }

    /// <summary>
    /// �������Ŷ�
    /// </summary>
    public QualityScore WithConfidence(decimal confidence)
    {
        return new QualityScore(
            AdId,
            RelevanceScore,
            UserExperienceScore,
            ExpectedCtr,
            ExpectedCvr,
            HistoricalPerformanceScore,
            CreativeQualityScore,
            LandingPageQualityScore,
            Version,
            confidence,
            CalculatedAt);
    }

    /// <summary>
    /// �Ƿ�Ϊ���������
    /// </summary>
    public bool IsHighQuality => OverallScore >= 7.0m;

    /// <summary>
    /// �Ƿ�Ϊ���������
    /// </summary>
    public bool IsLowQuality => OverallScore < 3.0m;

    /// <summary>
    /// ��ȡ��Ȩ������
    /// </summary>
    public decimal GetWeightedScore(decimal confidenceWeight = 1.0m)
    {
        return OverallScore * Confidence * confidenceWeight;
    }

    /// <summary>
    /// ��������������
    /// </summary>
    private decimal CalculateOverallScore()
    {
        // Ȩ�ط��䣺�����30%���û�����25%����ʷ����20%����������15%�����ҳ����10%
        var weightedScore =
            RelevanceScore * 0.30m +
            UserExperienceScore * 0.25m +
            HistoricalPerformanceScore * 0.20m +
            CreativeQualityScore * 0.15m +
            LandingPageQualityScore * 0.10m;

        return Math.Round(weightedScore, 2);
    }

    /// <summary>
    /// ��ȡ�ȼ��ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AdId;
        yield return OverallScore;
        yield return RelevanceScore;
        yield return UserExperienceScore;
        yield return ExpectedCtr;
        yield return ExpectedCvr;
        yield return HistoricalPerformanceScore;
        yield return CreativeQualityScore;
        yield return LandingPageQualityScore;
        yield return Version ?? string.Empty;
        yield return Confidence;
        yield return CalculatedAt;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInput(
        Guid adId,
        decimal relevanceScore,
        decimal userExperienceScore,
        decimal expectedCtr,
        decimal expectedCvr,
        decimal historicalPerformanceScore,
        decimal creativeQualityScore,
        decimal landingPageQualityScore,
        decimal confidence)
    {
        if (adId == Guid.Empty)
            throw new ArgumentException("���ID����Ϊ��", nameof(adId));

        ValidateScore(relevanceScore, nameof(relevanceScore));
        ValidateScore(userExperienceScore, nameof(userExperienceScore));
        ValidateScore(historicalPerformanceScore, nameof(historicalPerformanceScore));
        ValidateScore(creativeQualityScore, nameof(creativeQualityScore));
        ValidateScore(landingPageQualityScore, nameof(landingPageQualityScore));

        if (expectedCtr < 0 || expectedCtr > 1)
            throw new ArgumentException("Ԥ�ڵ���ʱ�����0-1֮��", nameof(expectedCtr));

        if (expectedCvr < 0 || expectedCvr > 1)
            throw new ArgumentException("Ԥ��ת���ʱ�����0-1֮��", nameof(expectedCvr));

        if (confidence < 0 || confidence > 1)
            throw new ArgumentException("���Ŷȱ�����0-1֮��", nameof(confidence));
    }

    /// <summary>
    /// ��֤���ַ�Χ
    /// </summary>
    private static void ValidateScore(decimal score, string paramName)
    {
        if (score < 0 || score > 10)
            throw new ArgumentException($"{paramName}������0-10֮��", paramName);
    }
}

