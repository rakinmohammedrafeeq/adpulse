using AdPulse.Core.Domain.Targeting;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// �û���Ȥ��������
    /// ʵ�ֻ����û���Ȥ�Ķ����������
    /// </summary>
    public class UserInterestTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "��Ȥ����";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Interest";

        /// <summary>
        /// Ŀ����Ȥ���
        /// </summary>
        public IReadOnlyList<string> TargetCategories => GetRule<List<string>>("TargetCategories") ?? new List<string>();

        /// <summary>
        /// Ŀ����Ȥ��ǩ
        /// </summary>
        public IReadOnlyList<string> TargetTags => GetRule<List<string>>("TargetTags") ?? new List<string>();

        /// <summary>
        /// ��С��Ȥ������ֵ (0-1)
        /// </summary>
        public decimal MinScoreThreshold => GetRule("MinScoreThreshold", 0.0m);

        /// <summary>
        /// ��С���Ŷ���ֵ (0-1)
        /// </summary>
        public decimal MinConfidenceThreshold => GetRule("MinConfidenceThreshold", 0.0m);

        /// <summary>
        /// �����ƥ��ģʽ���Ƿ���������
        /// </summary>
        public bool IncludeSubCategories => GetRule("IncludeSubCategories", true);

        /// <summary>
        /// ���캯��
        /// </summary>
        public UserInterestTargeting(
            IList<string>? targetCategories = null,
            IList<string>? targetTags = null,
            decimal minScoreThreshold = 0.0m,
            decimal minConfidenceThreshold = 0.0m,
            bool includeSubCategories = true,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetCategories, targetTags, minScoreThreshold, minConfidenceThreshold, includeSubCategories), weight, isEnabled)
        {
        }

        /// <summary>
        /// �������򼯺�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<string>? targetCategories,
            IList<string>? targetTags,
            decimal minScoreThreshold,
            decimal minConfidenceThreshold,
            bool includeSubCategories)
        {
            var rules = new List<TargetingRule>();

            if (targetCategories != null && targetCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetCategories",
                    System.Text.Json.JsonSerializer.Serialize(targetCategories),
                    "Json",
                    "Interest",
                    true,
                    1.0m,
                    "in",
                    "Ŀ����Ȥ���"));
            }

            if (targetTags != null && targetTags.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTags",
                    System.Text.Json.JsonSerializer.Serialize(targetTags),
                    "Json",
                    "Interest",
                    false,
                    1.0m,
                    "in",
                    "Ŀ����Ȥ��ǩ"));
            }

            if (minScoreThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinScoreThreshold",
                    minScoreThreshold.ToString(),
                    "Decimal",
                    "Interest",
                    false,
                    1.0m,
                    "gte",
                    "��С��Ȥ������ֵ"));
            }

            if (minConfidenceThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConfidenceThreshold",
                    minConfidenceThreshold.ToString(),
                    "Decimal",
                    "Interest",
                    false,
                    1.0m,
                    "gte",
                    "��С���Ŷ���ֵ"));
            }

            rules.Add(new TargetingRule(
                "IncludeSubCategories",
                includeSubCategories.ToString(),
                "Boolean",
                "Interest",
                false,
                1.0m,
                "eq",
                "�Ƿ���������"));

            return rules;
        }

        /// <summary>
        /// ������Ȥ��������
        /// </summary>
        public static UserInterestTargeting Create(
            IList<string>? targetCategories = null,
            IList<string>? targetTags = null,
            decimal minScoreThreshold = 0.0m,
            decimal minConfidenceThreshold = 0.0m,
            bool includeSubCategories = true,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserInterestTargeting(targetCategories, targetTags, minScoreThreshold, minConfidenceThreshold, includeSubCategories, weight, isEnabled);
        }

        /// <summary>
        /// ��������������Ȥ����
        /// </summary>
        public static UserInterestTargeting CreateByCategories(
            IList<string> categories,
            decimal minScoreThreshold = 0.3m,
            bool includeSubCategories = true,
            decimal weight = 1.0m)
        {
            return new UserInterestTargeting(
                targetCategories: categories,
                minScoreThreshold: minScoreThreshold,
                includeSubCategories: includeSubCategories,
                weight: weight);
        }

        /// <summary>
        /// �������ڱ�ǩ����Ȥ����
        /// </summary>
        public static UserInterestTargeting CreateByTags(
            IList<string> tags,
            decimal minConfidenceThreshold = 0.5m,
            decimal weight = 1.0m)
        {
            return new UserInterestTargeting(
                targetTags: tags,
                minConfidenceThreshold: minConfidenceThreshold,
                weight: weight);
        }

        /// <summary>
        /// �����Ȥ���
        /// </summary>
        /// <param name="category">��Ȥ���</param>
        public void AddTargetCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("��Ȥ�����Ϊ��", nameof(category));

            var currentList = TargetCategories.ToList();
            if (!currentList.Contains(category))
            {
                currentList.Add(category);
                SetRule("TargetCategories", currentList);
            }
        }

        /// <summary>
        /// �����Ȥ��ǩ
        /// </summary>
        /// <param name="tag">��Ȥ��ǩ</param>
        public void AddTargetTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                throw new ArgumentException("��Ȥ��ǩ����Ϊ��", nameof(tag));

            var currentList = TargetTags.ToList();
            if (!currentList.Contains(tag))
            {
                currentList.Add(tag);
                SetRule("TargetTags", currentList);
            }
        }

        /// <summary>
        /// ���·�����ֵ
        /// </summary>
        /// <param name="threshold">�µķ�����ֵ</param>
        public void UpdateScoreThreshold(decimal threshold)
        {
            if (threshold < 0 || threshold > 1)
                throw new ArgumentException("������ֵ������0-1֮��", nameof(threshold));

            SetRule("MinScoreThreshold", threshold);
        }

        /// <summary>
        /// �������Ŷ���ֵ
        /// </summary>
        /// <param name="threshold">�µ����Ŷ���ֵ</param>
        public void UpdateConfidenceThreshold(decimal threshold)
        {
            if (threshold < 0 || threshold > 1)
                throw new ArgumentException("���Ŷ���ֵ������0-1֮��", nameof(threshold));

            SetRule("MinConfidenceThreshold", threshold);
        }

        /// <summary>
        /// ��֤��Ȥ�����ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ������Ҫ����һ��Ŀ�������ǩ
            if (!TargetCategories.Any() && !TargetTags.Any())
                return false;

            // ��֤���ͱ�ǩ����Ϊ���ַ���
            if (TargetCategories.Any(string.IsNullOrWhiteSpace) || TargetTags.Any(string.IsNullOrWhiteSpace))
                return false;

            // ��֤��ֵ��Χ
            if (MinScoreThreshold < 0 || MinScoreThreshold > 1)
                return false;

            if (MinConfidenceThreshold < 0 || MinConfidenceThreshold > 1)
                return false;

            return true;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"Categories: {TargetCategories.Count}, Tags: {TargetTags.Count}, MinScore: {MinScoreThreshold:F2}, MinConfidence: {MinConfidenceThreshold:F2}";
            return $"{summary} - {details}";
        }
    }
}