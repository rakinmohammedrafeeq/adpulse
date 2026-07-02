using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects;

/// <summary>
/// ����ƥ����ֵ����
/// ��ʾ���������������û������ĵ�ƥ�����������OverallMatchResult��IndividualResults���ϵ�Ԫ������
/// </summary>
public class MatchResult : ValueObject
{
    /// <summary>
    /// �������ͣ������������ͣ���"geo"��"demographic"�ȣ�
    /// </summary>
    public string CriteriaType { get; private set; }

    /// <summary>
    /// ��������ʵ����ʶ
    /// </summary>
    public Guid CriteriaId { get; private set; }

    /// <summary>
    /// �Ƿ�ƥ��
    /// </summary>
    public bool IsMatch { get; private set; }

    /// <summary>
    /// ƥ��ȷ�����0-1��
    /// </summary>
    public decimal MatchScore { get; private set; }

    /// <summary>
    /// ƥ��ԭ��
    /// </summary>
    public string MatchReason { get; private set; }

    /// <summary>
    /// ��ƥ��ԭ��
    /// </summary>
    public string NotMatchReason { get; private set; }

    /// <summary>
    /// ��ϸƥ����Ϣ
    /// </summary>
    public IReadOnlyList<ContextProperty> MatchDetails { get; private set; }

    /// <summary>
    /// �������������ʱ
    /// </summary>
    public TimeSpan ExecutionTime { get; private set; }

    /// <summary>
    /// ����ʱ���
    /// </summary>
    public DateTime CalculatedAt { get; private set; }

    /// <summary>
    /// ���ȼ�
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Ȩ��
    /// </summary>
    public decimal Weight { get; private set; }

    /// <summary>
    /// �Ƿ�Ϊ��ѡ����
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// ˽�й��캯��
    /// </summary>
    private MatchResult(
        string criteriaType,
        Guid criteriaId,
        bool isMatch,
        decimal matchScore,
        string matchReason,
        string notMatchReason,
        IReadOnlyList<ContextProperty> matchDetails,
        TimeSpan executionTime,
        DateTime calculatedAt,
        int priority,
        decimal weight,
        bool isRequired)
    {
        CriteriaType = criteriaType;
        CriteriaId = criteriaId;
        IsMatch = isMatch;
        MatchScore = matchScore;
        MatchReason = matchReason;
        NotMatchReason = notMatchReason;
        MatchDetails = matchDetails;
        ExecutionTime = executionTime;
        CalculatedAt = calculatedAt;
        Priority = priority;
        Weight = weight;
        IsRequired = isRequired;
    }

    /// <summary>
    /// ����ƥ��ɹ��Ľ��
    /// </summary>
    public static MatchResult CreateMatch(
        string criteriaType,
        Guid criteriaId,
        decimal matchScore,
        string matchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IEnumerable<ContextProperty>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, matchScore, weight);

        var details = matchDetails?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchResult(
            criteriaType,
            criteriaId,
            true,
            matchScore,
            matchReason,
            string.Empty,
            details,
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// ����ƥ��ʧ�ܵĽ��
    /// </summary>
    public static MatchResult CreateNoMatch(
        string criteriaType,
        Guid criteriaId,
        string notMatchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IEnumerable<ContextProperty>? matchDetails = null)
    {
        ValidateInputs(criteriaType, criteriaId, 0m, weight);

        var details = matchDetails?.ToList().AsReadOnly() ?? new List<ContextProperty>().AsReadOnly();

        return new MatchResult(
            criteriaType,
            criteriaId,
            false,
            0m,
            string.Empty,
            notMatchReason,
            details,
            executionTime,
            DateTime.UtcNow,
            priority,
            weight,
            isRequired);
    }

    /// <summary>
    /// ����ƥ���������ֵ������
    /// </summary>
    public static MatchResult CreateMatchFromDictionary(
        string criteriaType,
        Guid criteriaId,
        decimal matchScore,
        string matchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        var details = ConvertDictionaryToContextProperties(matchDetails);
        return CreateMatch(criteriaType, criteriaId, matchScore, matchReason, executionTime, priority, weight, isRequired, details);
    }

    /// <summary>
    /// ������ƥ���������ֵ������
    /// </summary>
    public static MatchResult CreateNoMatchFromDictionary(
        string criteriaType,
        Guid criteriaId,
        string notMatchReason,
        TimeSpan executionTime,
        int priority = 0,
        decimal weight = 1.0m,
        bool isRequired = false,
        IDictionary<string, object>? matchDetails = null)
    {
        var details = ConvertDictionaryToContextProperties(matchDetails);
        return CreateNoMatch(criteriaType, criteriaId, notMatchReason, executionTime, priority, weight, isRequired, details);
    }

    /// <summary>
    /// ���ֵ�ת��Ϊ ContextProperty ����
    /// </summary>
    private static IEnumerable<ContextProperty> ConvertDictionaryToContextProperties(IDictionary<string, object>? dictionary)
    {
        if (dictionary == null)
            return Enumerable.Empty<ContextProperty>();

        return dictionary.Select(kvp =>
        {
            string propertyValue;
            string dataType;

            if (kvp.Value is string stringValue)
            {
                propertyValue = stringValue;
                dataType = "String";
            }
            else if (kvp.Value.GetType().IsPrimitive || kvp.Value is decimal || kvp.Value is DateTime)
            {
                propertyValue = kvp.Value.ToString() ?? string.Empty;
                dataType = kvp.Value.GetType().Name;
            }
            else
            {
                propertyValue = System.Text.Json.JsonSerializer.Serialize(kvp.Value);
                dataType = "Json";
            }

            return new ContextProperty(
                kvp.Key,
                propertyValue,
                dataType,
                "MatchDetail",
                false,
                1.0m,
                null,
                "MatchResult");
        });
    }

    /// <summary>
    /// ��ȡ��Ȩ����
    /// </summary>
    public decimal GetWeightedScore()
    {
        return MatchScore * Weight;
    }

    /// <summary>
    /// ��ȡִ��ָ��
    /// </summary>
    public Dictionary<string, object> GetExecutionMetrics()
    {
        return new Dictionary<string, object>
        {
            ["CriteriaType"] = CriteriaType,
            ["CriteriaId"] = CriteriaId,
            ["IsMatch"] = IsMatch,
            ["MatchScore"] = MatchScore,
            ["Weight"] = Weight,
            ["WeightedScore"] = GetWeightedScore(),
            ["ExecutionTime"] = ExecutionTime.TotalMilliseconds,
            ["Priority"] = Priority,
            ["IsRequired"] = IsRequired
        };
    }

    /// <summary>
    /// ����Ƿ���ָ������
    /// </summary>
    public bool HasDetail(string key)
    {
        return MatchDetails.Any(p => p.PropertyKey == key);
    }

    /// <summary>
    /// ��ȡָ������
    /// </summary>
    public T? GetDetail<T>(string key) where T : struct
    {
        var detail = MatchDetails.FirstOrDefault(p => p.PropertyKey == key);
        return detail?.GetValue<T>();
    }

    /// <summary>
    /// ��ȡָ�����飨�������ͣ�
    /// </summary>
    public T? GetDetailRef<T>(string key) where T : class
    {
        var detail = MatchDetails.FirstOrDefault(p => p.PropertyKey == key);
        return detail?.GetValue<T>();
    }

    /// <summary>
    /// ��ȡ����������Ϊ�ֵ䣨�����ݣ�
    /// </summary>
    public Dictionary<string, object> GetDetailsAsDictionary()
    {
        var result = new Dictionary<string, object>();
        foreach (var detail in MatchDetails)
        {
            try
            {
                var value = detail.GetValue<object>();
                if (value != null)
                {
                    result[detail.PropertyKey] = value;
                }
            }
            catch
            {
                // ���ת��ʧ�ܣ�ʹ��ԭʼ�ַ���ֵ
                result[detail.PropertyKey] = detail.PropertyValue;
            }
        }
        return result;
    }

    /// <summary>
    /// �Ƿ�Ϊ��Ч���
    /// </summary>
    public bool IsValidResult()
    {
        return !string.IsNullOrEmpty(CriteriaType) &&
               CriteriaId != Guid.Empty &&
               (IsMatch ? !string.IsNullOrEmpty(MatchReason) : !string.IsNullOrEmpty(NotMatchReason));
    }

    /// <summary>
    /// ��ȡ������Ϣ
    /// </summary>
    public string GetDebugInfo()
    {
        var status = IsMatch ? "MATCH" : "NO_MATCH";
        var reason = IsMatch ? MatchReason : NotMatchReason;
        var details = MatchDetails.Any()
            ? string.Join(", ", MatchDetails.Take(3).Select(detail => $"{detail.PropertyKey}:{detail.PropertyValue}"))
            : "No Details";

        return $"{CriteriaType}[{CriteriaId}]: {status} Score:{MatchScore:F3} Weight:{Weight:F2} " +
               $"Reason:{reason} Time:{ExecutionTime.TotalMilliseconds:F1}ms Details:[{details}]";
    }

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CriteriaType;
        yield return CriteriaId;
        yield return IsMatch;
        yield return MatchScore;
        yield return Weight;
        yield return Priority;
        yield return IsRequired;
        yield return CalculatedAt;
    }

    /// <summary>
    /// ��֤�������
    /// </summary>
    private static void ValidateInputs(string criteriaType, Guid criteriaId, decimal matchScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(criteriaType))
            throw new ArgumentException("�������Ͳ���Ϊ��", nameof(criteriaType));

        if (criteriaId == Guid.Empty)
            throw new ArgumentException("����ID����Ϊ��", nameof(criteriaId));

        if (matchScore < 0m || matchScore > 1m)
            throw new ArgumentException("ƥ�����������0-1֮��", nameof(matchScore));

        if (weight < 0m)
            throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
    }
}