using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Domain.Enums;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// �û�ƫ�ö�������
    /// ʵ�ֻ����û�ƫ�ú���˽���õĶ����������
    /// </summary>
    public class UserPreferenceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "ƫ�ö���";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Preference";

        /// <summary>
        /// Ŀ��������
        /// </summary>
        public IReadOnlyList<AdType> TargetAdTypes => GetRule<List<AdType>>("TargetAdTypes") ?? new List<AdType>();

        /// <summary>
        /// �ų����������
        /// </summary>
        public IReadOnlyList<string> ExcludedCategories => GetRule<List<string>>("ExcludedCategories") ?? new List<string>();

        /// <summary>
        /// Ŀ�껰��
        /// </summary>
        public IReadOnlyList<string> TargetTopics => GetRule<List<string>>("TargetTopics") ?? new List<string>();

        /// <summary>
        /// Ŀ������
        /// </summary>
        public IReadOnlyList<string> TargetLanguages => GetRule<List<string>>("TargetLanguages") ?? new List<string>();

        /// <summary>
        /// �����˽����Ҫ��
        /// </summary>
        public PrivacyLevel MaxPrivacyLevel => GetRule("MaxPrivacyLevel", PrivacyLevel.Maximum);

        /// <summary>
        /// ������ݳ���ȼ���
        /// </summary>
        public ContentMaturityLevel MaxContentMaturityLevel => GetRule("MaxContentMaturityLevel", ContentMaturityLevel.Restricted);

        /// <summary>
        /// ���ÿ�չ��չʾ����
        /// </summary>
        public int? MaxDailyImpressions => GetRule<int?>("MaxDailyImpressions");

        /// <summary>
        /// �Ƿ�Ҫ��������Ի����
        /// </summary>
        public bool RequirePersonalizedAdsConsent => GetRule("RequirePersonalizedAdsConsent", true);

        /// <summary>
        /// �Ƿ�Ҫ��������Ϊ׷��
        /// </summary>
        public bool RequireBehaviorTrackingConsent => GetRule("RequireBehaviorTrackingConsent", false);

        /// <summary>
        /// �Ƿ�Ҫ��������豸׷��
        /// </summary>
        public bool RequireCrossDeviceTrackingConsent => GetRule("RequireCrossDeviceTrackingConsent", false);

        /// <summary>
        /// ���캯��
        /// </summary>
        public UserPreferenceTargeting(
            IList<AdType>? targetAdTypes = null,
            IList<string>? excludedCategories = null,
            IList<string>? targetTopics = null,
            IList<string>? targetLanguages = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Maximum,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.Restricted,
            int? maxDailyImpressions = null,
            bool requirePersonalizedAdsConsent = true,
            bool requireBehaviorTrackingConsent = false,
            bool requireCrossDeviceTrackingConsent = false,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(targetAdTypes, excludedCategories, targetTopics, targetLanguages, maxPrivacyLevel, maxContentMaturityLevel, maxDailyImpressions, requirePersonalizedAdsConsent, requireBehaviorTrackingConsent, requireCrossDeviceTrackingConsent), weight, isEnabled)
        {
        }

        /// <summary>
        /// �������򼯺�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<AdType>? targetAdTypes,
            IList<string>? excludedCategories,
            IList<string>? targetTopics,
            IList<string>? targetLanguages,
            PrivacyLevel maxPrivacyLevel,
            ContentMaturityLevel maxContentMaturityLevel,
            int? maxDailyImpressions,
            bool requirePersonalizedAdsConsent,
            bool requireBehaviorTrackingConsent,
            bool requireCrossDeviceTrackingConsent)
        {
            var rules = new List<TargetingRule>();

            if (targetAdTypes != null && targetAdTypes.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetAdTypes",
                    System.Text.Json.JsonSerializer.Serialize(targetAdTypes.Select(t => (int)t)),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "Ŀ��������"));
            }

            if (excludedCategories != null && excludedCategories.Any())
            {
                rules.Add(new TargetingRule(
                    "ExcludedCategories",
                    System.Text.Json.JsonSerializer.Serialize(excludedCategories),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "not_in",
                    "�ų����������"));
            }

            if (targetTopics != null && targetTopics.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetTopics",
                    System.Text.Json.JsonSerializer.Serialize(targetTopics),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "Ŀ�껰��"));
            }

            if (targetLanguages != null && targetLanguages.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetLanguages",
                    System.Text.Json.JsonSerializer.Serialize(targetLanguages),
                    "Json",
                    "Preference",
                    false,
                    1.0m,
                    "in",
                    "Ŀ������"));
            }

            rules.Add(new TargetingRule(
                "MaxPrivacyLevel",
                ((int)maxPrivacyLevel).ToString(),
                "Integer",
                "Preference",
                true,
                1.0m,
                "lte",
                "�����˽����"));

            rules.Add(new TargetingRule(
                "MaxContentMaturityLevel",
                ((int)maxContentMaturityLevel).ToString(),
                "Integer",
                "Preference",
                false,
                1.0m,
                "lte",
                "������ݳ���ȼ���"));

            if (maxDailyImpressions.HasValue)
            {
                rules.Add(new TargetingRule(
                    "MaxDailyImpressions",
                    maxDailyImpressions.Value.ToString(),
                    "Integer",
                    "Preference",
                    false,
                    1.0m,
                    "lte",
                    "���ÿ�չ��չʾ����"));
            }

            rules.Add(new TargetingRule(
                "RequirePersonalizedAdsConsent",
                requirePersonalizedAdsConsent.ToString(),
                "Boolean",
                "Preference",
                true,
                1.0m,
                "eq",
                "�Ƿ�Ҫ����Ի����ͬ��"));

            if (requireBehaviorTrackingConsent)
            {
                rules.Add(new TargetingRule(
                    "RequireBehaviorTrackingConsent",
                    requireBehaviorTrackingConsent.ToString(),
                    "Boolean",
                    "Preference",
                    true,
                    1.0m,
                    "eq",
                    "�Ƿ�Ҫ����Ϊ׷��ͬ��"));
            }

            if (requireCrossDeviceTrackingConsent)
            {
                rules.Add(new TargetingRule(
                    "RequireCrossDeviceTrackingConsent",
                    requireCrossDeviceTrackingConsent.ToString(),
                    "Boolean",
                    "Preference",
                    true,
                    1.0m,
                    "eq",
                    "�Ƿ�Ҫ����豸׷��ͬ��"));
            }

            return rules;
        }

        /// <summary>
        /// ����ƫ�ö�������
        /// </summary>
        public static UserPreferenceTargeting Create(
            IList<AdType>? targetAdTypes = null,
            IList<string>? excludedCategories = null,
            IList<string>? targetTopics = null,
            IList<string>? targetLanguages = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Maximum,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.Restricted,
            int? maxDailyImpressions = null,
            bool requirePersonalizedAdsConsent = true,
            bool requireBehaviorTrackingConsent = false,
            bool requireCrossDeviceTrackingConsent = false,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserPreferenceTargeting(targetAdTypes, excludedCategories, targetTopics, targetLanguages, maxPrivacyLevel, maxContentMaturityLevel, maxDailyImpressions, requirePersonalizedAdsConsent, requireBehaviorTrackingConsent, requireCrossDeviceTrackingConsent, weight, isEnabled);
        }

        /// <summary>
        /// ������˽�Ѻõ�ƫ�ö���
        /// </summary>
        public static UserPreferenceTargeting CreatePrivacyFriendly(
            IList<AdType>? targetAdTypes = null,
            PrivacyLevel maxPrivacyLevel = PrivacyLevel.Standard,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetAdTypes: targetAdTypes,
                maxPrivacyLevel: maxPrivacyLevel,
                requirePersonalizedAdsConsent: false,
                requireBehaviorTrackingConsent: false,
                requireCrossDeviceTrackingConsent: false,
                weight: weight);
        }

        /// <summary>
        /// ������������ƫ�õĶ���
        /// </summary>
        public static UserPreferenceTargeting CreateContentBased(
            IList<string> targetTopics,
            IList<string>? excludedCategories = null,
            ContentMaturityLevel maxContentMaturityLevel = ContentMaturityLevel.General,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetTopics: targetTopics,
                excludedCategories: excludedCategories,
                maxContentMaturityLevel: maxContentMaturityLevel,
                weight: weight);
        }

        /// <summary>
        /// �����������ԵĶ���
        /// </summary>
        public static UserPreferenceTargeting CreateLanguageBased(
            IList<string> targetLanguages,
            decimal weight = 1.0m)
        {
            return new UserPreferenceTargeting(
                targetLanguages: targetLanguages,
                weight: weight);
        }

        /// <summary>
        /// ���Ŀ��������
        /// </summary>
        /// <param name="adType">�������</param>
        public void AddTargetAdType(AdType adType)
        {
            var currentList = TargetAdTypes.ToList();
            if (!currentList.Contains(adType))
            {
                currentList.Add(adType);
                SetRule("TargetAdTypes", currentList.Select(t => (int)t).ToList());
            }
        }

        /// <summary>
        /// ����ų����������
        /// </summary>
        /// <param name="category">�������</param>
        public void AddExcludedCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("���������Ϊ��", nameof(category));

            var currentList = ExcludedCategories.ToList();
            if (!currentList.Contains(category))
            {
                currentList.Add(category);
                SetRule("ExcludedCategories", currentList);
            }
        }

        /// <summary>
        /// ���Ŀ�껰��
        /// </summary>
        /// <param name="topic">����</param>
        public void AddTargetTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("���ⲻ��Ϊ��", nameof(topic));

            var currentList = TargetTopics.ToList();
            if (!currentList.Contains(topic))
            {
                currentList.Add(topic);
                SetRule("TargetTopics", currentList);
            }
        }

        /// <summary>
        /// ���Ŀ������
        /// </summary>
        /// <param name="language">����</param>
        public void AddTargetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException("���Բ���Ϊ��", nameof(language));

            var currentList = TargetLanguages.ToList();
            if (!currentList.Contains(language))
            {
                currentList.Add(language);
                SetRule("TargetLanguages", currentList);
            }
        }

        /// <summary>
        /// ������˽����Ҫ��
        /// </summary>
        /// <param name="privacyLevel">��˽����</param>
        public void UpdatePrivacyLevel(PrivacyLevel privacyLevel)
        {
            SetRule("MaxPrivacyLevel", (int)privacyLevel);
        }

        /// <summary>
        /// �������ݳ���ȼ���Ҫ��
        /// </summary>
        /// <param name="maturityLevel">���ݳ���ȼ���</param>
        public void UpdateContentMaturityLevel(ContentMaturityLevel maturityLevel)
        {
            SetRule("MaxContentMaturityLevel", (int)maturityLevel);
        }

        /// <summary>
        /// ����ÿ��չʾ��������
        /// </summary>
        /// <param name="maxImpressions">���չʾ����</param>
        public void SetDailyImpressionsLimit(int? maxImpressions)
        {
            if (maxImpressions.HasValue && maxImpressions.Value < 0)
                throw new ArgumentException("ÿ��չʾ��������Ϊ����", nameof(maxImpressions));

            if (maxImpressions.HasValue)
                SetRule("MaxDailyImpressions", maxImpressions.Value);
            else
                RemoveRule("MaxDailyImpressions");
        }

        /// <summary>
        /// ��֤ƫ�ö����ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ��֤�ַ����б���ܰ�����ֵ
            if (ExcludedCategories.Any(string.IsNullOrWhiteSpace) ||
                TargetTopics.Any(string.IsNullOrWhiteSpace) ||
                TargetLanguages.Any(string.IsNullOrWhiteSpace))
                return false;

            // ��֤ÿ��չʾ��������
            if (MaxDailyImpressions.HasValue && MaxDailyImpressions.Value < 0)
                return false;

            return true;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"AdTypes: {TargetAdTypes.Count}, Topics: {TargetTopics.Count}, Languages: {TargetLanguages.Count}, Privacy: {MaxPrivacyLevel}, Content: {MaxContentMaturityLevel}";
            return $"{summary} - {details}";
        }
    }
}