using AdPulse.Core.Domain.Targeting;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// �û���ǩ��������
    /// ʵ�ֻ����û���ǩ�Ķ����������
    /// </summary>
    public class UserTagTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "��ǩ����";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Tag";

        /// <summary>
        /// Ŀ���ǩ�����б�
        /// </summary>
        public IReadOnlyList<string> TargetTagNames => GetRule<List<string>>("TargetTagNames") ?? new List<string>();

        /// <summary>
        /// Ŀ���ǩ�����б�
        /// </summary>
        public IReadOnlyList<string> TargetTagTypes => GetRule<List<string>>("TargetTagTypes") ?? new List<string>();

        /// <summary>
        /// Ŀ���ǩ�����б�
        /// </summary>
        public IReadOnlyList<string> TargetCategories => GetRule<List<string>>("TargetCategories") ?? new List<string>();

        /// <summary>
        /// �ų��ı�ǩ�����б�
        /// </summary>
        public IReadOnlyList<string> ExcludedTagNames => GetRule<List<string>>("ExcludedTagNames") ?? new List<string>();

        /// <summary>
        /// ��С���Ŷ���ֵ (0-1)
        /// </summary>
        public decimal MinConfidenceThreshold => GetRule("MinConfidenceThreshold", 0.0m);

        /// <summary>
        /// ��СȨ����ֵ
        /// </summary>
        public decimal MinWeightThreshold => GetRule("MinWeightThreshold", 0.0m);

        /// <summary>
        /// �Ƿ�����ѹ��ڱ�ǩ
        /// </summary>
        public bool IncludeExpiredTags => GetRule("IncludeExpiredTags", false);

        /// <summary>
        /// ��ǩƥ��ģʽ��Any=����ƥ��, All=ȫ��ƥ�䣩
        /// </summary>
        public string MatchMode => GetRule("MatchMode", "Any");

        /// <summary>
        /// ���캯��
        /// </summary>
        public UserTagTargeting(
            IList<string>? targetTagNames = null,
            IList<string>? targetTagTypes = null,
            IList<string>? targetCategories = null,
            IList<string>? excludedTagNames = null,
            decimal minConfidenceThreshold = 0.0m,
            decimal minWeightThreshold = 0.0m,
            bool includeExpiredTags = false,
            string matchMode = "Any",
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetTagNames, targetTagTypes, targetCategories, excludedTagNames, minConfidenceThreshold, minWeightThreshold, includeExpiredTags, matchMode), weight, isEnabled)
        {
        }

        /// <summary>
        /// �������򼯺�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<string>? targetTagNames,
            IList<string>? targetTagTypes,
            IList<string>? targetCategories,
            IList<string>? excludedTagNames,
            decimal minConfidenceThreshold,
            decimal minWeightThreshold,
            bool includeExpiredTags,
            string matchMode)
        {
            var rules = new List<TargetingRule>();

            if (targetTagNames != null && targetTagNames.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTagNames",
                    System.Text.Json.JsonSerializer.Serialize(targetTagNames),
                    "Json",
                    "Tag",
                    true,
                    1.0m,
                    "in",
                    "Ŀ���ǩ����"));
            }

            if (targetTagTypes != null && targetTagTypes.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTagTypes",
                    System.Text.Json.JsonSerializer.Serialize(targetTagTypes),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "in",
                    "Ŀ���ǩ����"));
            }

            if (targetCategories != null && targetCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetCategories",
                    System.Text.Json.JsonSerializer.Serialize(targetCategories),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "in",
                    "Ŀ���ǩ����"));
            }

            if (excludedTagNames != null && excludedTagNames.Any())
            {
                rules.Add(new TargetingRule(
                    "ExcludedTagNames",
                    System.Text.Json.JsonSerializer.Serialize(excludedTagNames),
                    "Json",
                    "Tag",
                    false,
                    1.0m,
                    "not_in",
                    "�ų��ı�ǩ����"));
            }

            if (minConfidenceThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConfidenceThreshold",
                    minConfidenceThreshold.ToString(),
                    "Decimal",
                    "Tag",
                    false,
                    1.0m,
                    "gte",
                    "��С���Ŷ���ֵ"));
            }

            if (minWeightThreshold > 0)
            {
                rules.Add(new TargetingRule(
                    "MinWeightThreshold",
                    minWeightThreshold.ToString(),
                    "Decimal",
                    "Tag",
                    false,
                    1.0m,
                    "gte",
                    "��СȨ����ֵ"));
            }

            rules.Add(new TargetingRule(
                "IncludeExpiredTags",
                includeExpiredTags.ToString(),
                "Boolean",
                "Tag",
                false,
                1.0m,
                "eq",
                "�Ƿ�����ѹ��ڱ�ǩ"));

            rules.Add(new TargetingRule(
                "MatchMode",
                matchMode,
                "String",
                "Tag",
                false,
                1.0m,
                "eq",
                "��ǩƥ��ģʽ"));

            return rules;
        }

        /// <summary>
        /// ������ǩ��������
        /// </summary>
        public static UserTagTargeting Create(
            IList<string>? targetTagNames = null,
            IList<string>? targetTagTypes = null,
            IList<string>? targetCategories = null,
            IList<string>? excludedTagNames = null,
            decimal minConfidenceThreshold = 0.0m,
            decimal minWeightThreshold = 0.0m,
            bool includeExpiredTags = false,
            string matchMode = "Any",
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserTagTargeting(targetTagNames, targetTagTypes, targetCategories, excludedTagNames, minConfidenceThreshold, minWeightThreshold, includeExpiredTags, matchMode, weight, isEnabled);
        }

        /// <summary>
        /// �������ڱ�ǩ���ƵĶ���
        /// </summary>
        public static UserTagTargeting CreateByTagNames(
            IList<string> tagNames,
            decimal minConfidenceThreshold = 0.5m,
            string matchMode = "Any",
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagNames: tagNames,
                minConfidenceThreshold: minConfidenceThreshold,
                matchMode: matchMode,
                weight: weight);
        }

        /// <summary>
        /// �������ڱ�ǩ���͵Ķ���
        /// </summary>
        public static UserTagTargeting CreateByTagTypes(
            IList<string> tagTypes,
            decimal minConfidenceThreshold = 0.3m,
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagTypes: tagTypes,
                minConfidenceThreshold: minConfidenceThreshold,
                weight: weight);
        }

        /// <summary>
        /// �����ų��Ա�ǩ����
        /// </summary>
        public static UserTagTargeting CreateExclusive(
            IList<string> includeTags,
            IList<string> excludeTags,
            decimal weight = 1.0m)
        {
            return new UserTagTargeting(
                targetTagNames: includeTags,
                excludedTagNames: excludeTags,
                weight: weight);
        }

        /// <summary>
        /// ���Ŀ���ǩ����
        /// </summary>
        /// <param name="tagName">��ǩ����</param>
        public void AddTargetTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("��ǩ���Ʋ���Ϊ��", nameof(tagName));

            var currentList = TargetTagNames.ToList();
            if (!currentList.Contains(tagName))
            {
                currentList.Add(tagName);
                SetRule("TargetTagNames", currentList);
            }
        }

        /// <summary>
        /// ���Ŀ���ǩ����
        /// </summary>
        /// <param name="tagType">��ǩ����</param>
        public void AddTargetTagType(string tagType)
        {
            if (string.IsNullOrEmpty(tagType))
                throw new ArgumentException("��ǩ���Ͳ���Ϊ��", nameof(tagType));

            var currentList = TargetTagTypes.ToList();
            if (!currentList.Contains(tagType))
            {
                currentList.Add(tagType);
                SetRule("TargetTagTypes", currentList);
            }
        }

        /// <summary>
        /// ����ų��ı�ǩ����
        /// </summary>
        /// <param name="tagName">Ҫ�ų��ı�ǩ����</param>
        public void AddExcludedTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("��ǩ���Ʋ���Ϊ��", nameof(tagName));

            var currentList = ExcludedTagNames.ToList();
            if (!currentList.Contains(tagName))
            {
                currentList.Add(tagName);
                SetRule("ExcludedTagNames", currentList);
            }
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
        /// ����Ȩ����ֵ
        /// </summary>
        /// <param name="threshold">�µ�Ȩ����ֵ</param>
        public void UpdateWeightThreshold(decimal threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("Ȩ����ֵ����Ϊ����", nameof(threshold));

            SetRule("MinWeightThreshold", threshold);
        }

        /// <summary>
        /// ����ƥ��ģʽ
        /// </summary>
        /// <param name="mode">ƥ��ģʽ��Any/All��</param>
        public void SetMatchMode(string mode)
        {
            if (string.IsNullOrEmpty(mode) || (mode != "Any" && mode != "All"))
                throw new ArgumentException("ƥ��ģʽ������ 'Any' �� 'All'", nameof(mode));

            SetRule("MatchMode", mode);
        }

        /// <summary>
        /// ��֤��ǩ�����ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ������Ҫ����һ��Ŀ���ǩ����
            if (!TargetTagNames.Any() && !TargetTagTypes.Any() && !TargetCategories.Any())
                return false;

            // ��֤��ǩ���ơ����ͺͷ��಻��Ϊ���ַ���
            if (TargetTagNames.Any(string.IsNullOrWhiteSpace) || 
                TargetTagTypes.Any(string.IsNullOrWhiteSpace) ||
                TargetCategories.Any(string.IsNullOrWhiteSpace) ||
                ExcludedTagNames.Any(string.IsNullOrWhiteSpace))
                return false;

            // ��֤��ֵ��Χ
            if (MinConfidenceThreshold < 0 || MinConfidenceThreshold > 1)
                return false;

            if (MinWeightThreshold < 0)
                return false;

            // ��֤ƥ��ģʽ
            if (MatchMode != "Any" && MatchMode != "All")
                return false;

            return true;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"TagNames: {TargetTagNames.Count}, TagTypes: {TargetTagTypes.Count}, Categories: {TargetCategories.Count}, Excluded: {ExcludedTagNames.Count}, Mode: {MatchMode}";
            return $"{summary} - {details}";
        }
    }
}