using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Domain.Enums;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// �û���ֵ��������
    /// ʵ�ֻ����û���ֵ�����Ķ����������
    /// </summary>
    public class UserValueTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "��ֵ����";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Value";

        /// <summary>
        /// ��С��������� (0-100)
        /// </summary>
        public int MinEngagementScore => GetRule("MinEngagementScore", 0);

        /// <summary>
        /// ��С�ҳ϶����� (0-100)
        /// </summary>
        public int MinLoyaltyScore => GetRule("MinLoyaltyScore", 0);

        /// <summary>
        /// ��С���Ѽ�ֵ���� (0-100)
        /// </summary>
        public int MinMonetaryScore => GetRule("MinMonetaryScore", 0);

        /// <summary>
        /// ��СǱ������ (0-100)
        /// </summary>
        public int MinPotentialScore => GetRule("MinPotentialScore", 0);

        /// <summary>
        /// ��С�ۺ����� (0-100)
        /// </summary>
        public int MinOverallScore => GetRule("MinOverallScore", 0);

        /// <summary>
        /// ��СԤ���������ڼ�ֵ��Ԫ��
        /// </summary>
        public decimal MinEstimatedLTV => GetRule("MinEstimatedLTV", 0.0m);

        /// <summary>
        /// Ŀ�����������ȼ�
        /// </summary>
        public IReadOnlyList<SpendingLevel> TargetSpendingLevels => GetRule<List<SpendingLevel>>("TargetSpendingLevels") ?? new List<SpendingLevel>();

        /// <summary>
        /// Ŀ���û���ֵ�ȼ�
        /// </summary>
        public IReadOnlyList<ValueTier> TargetValueTiers => GetRule<List<ValueTier>>("TargetValueTiers") ?? new List<ValueTier>();

        /// <summary>
        /// ��Сת������ (0.0-1.0)
        /// </summary>
        public decimal MinConversionProbability => GetRule("MinConversionProbability", 0.0m);

        /// <summary>
        /// �Ƿ������߼�ֵ�û�
        /// </summary>
        public bool HighValueUsersOnly => GetRule("HighValueUsersOnly", false);

        /// <summary>
        /// �Ƿ�������Ծ�û�
        /// </summary>
        public bool ActiveUsersOnly => GetRule("ActiveUsersOnly", false);

        /// <summary>
        /// �Ƿ�������ҳ��û�
        /// </summary>
        public bool LoyalUsersOnly => GetRule("LoyalUsersOnly", false);

        /// <summary>
        /// ���캯��
        /// </summary>
        public UserValueTargeting(
            int minEngagementScore = 0,
            int minLoyaltyScore = 0,
            int minMonetaryScore = 0,
            int minPotentialScore = 0,
            int minOverallScore = 0,
            decimal minEstimatedLTV = 0.0m,
            IList<SpendingLevel>? targetSpendingLevels = null,
            IList<ValueTier>? targetValueTiers = null,
            decimal minConversionProbability = 0.0m,
            bool highValueUsersOnly = false,
            bool activeUsersOnly = false,
            bool loyalUsersOnly = false,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(minEngagementScore, minLoyaltyScore, minMonetaryScore, minPotentialScore, minOverallScore, minEstimatedLTV, targetSpendingLevels, targetValueTiers, minConversionProbability, highValueUsersOnly, activeUsersOnly, loyalUsersOnly), weight, isEnabled)
        {
        }

        /// <summary>
        /// �������򼯺�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            int minEngagementScore,
            int minLoyaltyScore,
            int minMonetaryScore,
            int minPotentialScore,
            int minOverallScore,
            decimal minEstimatedLTV,
            IList<SpendingLevel>? targetSpendingLevels,
            IList<ValueTier>? targetValueTiers,
            decimal minConversionProbability,
            bool highValueUsersOnly,
            bool activeUsersOnly,
            bool loyalUsersOnly)
        {
            var rules = new List<TargetingRule>();

            if (minEngagementScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinEngagementScore",
                    minEngagementScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��С���������"));
            }

            if (minLoyaltyScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinLoyaltyScore",
                    minLoyaltyScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��С�ҳ϶�����"));
            }

            if (minMonetaryScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinMonetaryScore",
                    minMonetaryScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��С���Ѽ�ֵ����"));
            }

            if (minPotentialScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinPotentialScore",
                    minPotentialScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��СǱ������"));
            }

            if (minOverallScore > 0)
            {
                rules.Add(new TargetingRule(
                    "MinOverallScore",
                    minOverallScore.ToString(),
                    "Integer",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��С�ۺ�����"));
            }

            if (minEstimatedLTV > 0)
            {
                rules.Add(new TargetingRule(
                    "MinEstimatedLTV",
                    minEstimatedLTV.ToString(),
                    "Decimal",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��СԤ���������ڼ�ֵ"));
            }

            if (targetSpendingLevels != null && targetSpendingLevels.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetSpendingLevels",
                    System.Text.Json.JsonSerializer.Serialize(targetSpendingLevels.Select(s => (int)s)),
                    "Json",
                    "Value",
                    false,
                    1.0m,
                    "in",
                    "Ŀ�����������ȼ�"));
            }

            if (targetValueTiers != null && targetValueTiers.Any())
            {
                rules.Add(new TargetingRule(
                    "TargetValueTiers",
                    System.Text.Json.JsonSerializer.Serialize(targetValueTiers.Select(v => (int)v)),
                    "Json",
                    "Value",
                    false,
                    1.0m,
                    "in",
                    "Ŀ���û���ֵ�ȼ�"));
            }

            if (minConversionProbability > 0)
            {
                rules.Add(new TargetingRule(
                    "MinConversionProbability",
                    minConversionProbability.ToString(),
                    "Decimal",
                    "Value",
                    false,
                    1.0m,
                    "gte",
                    "��Сת������"));
            }

            if (highValueUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "HighValueUsersOnly",
                    highValueUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "������߼�ֵ�û�"));
            }

            if (activeUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "ActiveUsersOnly",
                    activeUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "�������Ծ�û�"));
            }

            if (loyalUsersOnly)
            {
                rules.Add(new TargetingRule(
                    "LoyalUsersOnly",
                    loyalUsersOnly.ToString(),
                    "Boolean",
                    "Value",
                    false,
                    1.0m,
                    "eq",
                    "�������ҳ��û�"));
            }

            return rules;
        }

        /// <summary>
        /// ������ֵ��������
        /// </summary>
        public static UserValueTargeting Create(
            int minEngagementScore = 0,
            int minLoyaltyScore = 0,
            int minMonetaryScore = 0,
            int minPotentialScore = 0,
            int minOverallScore = 0,
            decimal minEstimatedLTV = 0.0m,
            IList<SpendingLevel>? targetSpendingLevels = null,
            IList<ValueTier>? targetValueTiers = null,
            decimal minConversionProbability = 0.0m,
            bool highValueUsersOnly = false,
            bool activeUsersOnly = false,
            bool loyalUsersOnly = false,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new UserValueTargeting(minEngagementScore, minLoyaltyScore, minMonetaryScore, minPotentialScore, minOverallScore, minEstimatedLTV, targetSpendingLevels, targetValueTiers, minConversionProbability, highValueUsersOnly, activeUsersOnly, loyalUsersOnly, weight, isEnabled);
        }

        /// <summary>
        /// �����߼�ֵ�û�����
        /// </summary>
        public static UserValueTargeting CreateHighValue(
            int minOverallScore = 80,
            IList<ValueTier>? targetValueTiers = null,
            decimal minEstimatedLTV = 500.0m,
            decimal weight = 1.5m)
        {
            var valueTiers = targetValueTiers ?? new List<ValueTier> { ValueTier.Premium, ValueTier.VIP };
            return new UserValueTargeting(
                minOverallScore: minOverallScore,
                targetValueTiers: valueTiers,
                minEstimatedLTV: minEstimatedLTV,
                highValueUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// �����������������Ķ���
        /// </summary>
        public static UserValueTargeting CreateBySpendingLevel(
            IList<SpendingLevel> targetSpendingLevels,
            int minMonetaryScore = 50,
            decimal weight = 1.0m)
        {
            return new UserValueTargeting(
                minMonetaryScore: minMonetaryScore,
                targetSpendingLevels: targetSpendingLevels,
                weight: weight);
        }

        /// <summary>
        /// ��������ת�����ʵĶ���
        /// </summary>
        public static UserValueTargeting CreateByConversionProbability(
            decimal minConversionProbability,
            int minPotentialScore = 60,
            decimal weight = 1.2m)
        {
            return new UserValueTargeting(
                minPotentialScore: minPotentialScore,
                minConversionProbability: minConversionProbability,
                weight: weight);
        }

        /// <summary>
        /// ������Ծ�û�����
        /// </summary>
        public static UserValueTargeting CreateActiveUsers(
            int minEngagementScore = 60,
            decimal weight = 1.0m)
        {
            return new UserValueTargeting(
                minEngagementScore: minEngagementScore,
                activeUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// �����ҳ��û�����
        /// </summary>
        public static UserValueTargeting CreateLoyalUsers(
            int minLoyaltyScore = 70,
            decimal weight = 1.1m)
        {
            return new UserValueTargeting(
                minLoyaltyScore: minLoyaltyScore,
                loyalUsersOnly: true,
                weight: weight);
        }

        /// <summary>
        /// ���Ŀ�����������ȼ�
        /// </summary>
        /// <param name="spendingLevel">���������ȼ�</param>
        public void AddTargetSpendingLevel(SpendingLevel spendingLevel)
        {
            var currentList = TargetSpendingLevels.ToList();
            if (!currentList.Contains(spendingLevel))
            {
                currentList.Add(spendingLevel);
                SetRule("TargetSpendingLevels", currentList.Select(s => (int)s).ToList());
            }
        }

        /// <summary>
        /// ���Ŀ���ֵ�ȼ�
        /// </summary>
        /// <param name="valueTier">��ֵ�ȼ�</param>
        public void AddTargetValueTier(ValueTier valueTier)
        {
            var currentList = TargetValueTiers.ToList();
            if (!currentList.Contains(valueTier))
            {
                currentList.Add(valueTier);
                SetRule("TargetValueTiers", currentList.Select(v => (int)v).ToList());
            }
        }

        /// <summary>
        /// ������С������ֵ
        /// </summary>
        /// <param name="scoreType">��������</param>
        /// <param name="minScore">��С����</param>
        public void UpdateMinScore(string scoreType, int minScore)
        {
            if (minScore < 0 || minScore > 100)
                throw new ArgumentException("���ֱ�����0-100֮��", nameof(minScore));

            var validScoreTypes = new[] { "EngagementScore", "LoyaltyScore", "MonetaryScore", "PotentialScore", "OverallScore" };
            if (!validScoreTypes.Contains(scoreType))
                throw new ArgumentException($"��Ч����������: {scoreType}", nameof(scoreType));

            SetRule($"Min{scoreType}", minScore);
        }

        /// <summary>
        /// ������С�������ڼ�ֵ
        /// </summary>
        /// <param name="minLTV">��С�������ڼ�ֵ</param>
        public void UpdateMinEstimatedLTV(decimal minLTV)
        {
            if (minLTV < 0)
                throw new ArgumentException("�������ڼ�ֵ����Ϊ����", nameof(minLTV));

            SetRule("MinEstimatedLTV", minLTV);
        }

        /// <summary>
        /// ������Сת������
        /// </summary>
        /// <param name="minProbability">��Сת������</param>
        public void UpdateMinConversionProbability(decimal minProbability)
        {
            if (minProbability < 0 || minProbability > 1)
                throw new ArgumentException("ת�����ʱ�����0-1֮��", nameof(minProbability));

            SetRule("MinConversionProbability", minProbability);
        }

        /// <summary>
        /// ��֤��ֵ�����ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ��֤���ַ�Χ
            var scores = new[] { MinEngagementScore, MinLoyaltyScore, MinMonetaryScore, MinPotentialScore, MinOverallScore };
            if (scores.Any(score => score < 0 || score > 100))
                return false;

            // ��֤ת�����ʷ�Χ
            if (MinConversionProbability < 0 || MinConversionProbability > 1)
                return false;

            // ��֤�������ڼ�ֵ
            if (MinEstimatedLTV < 0)
                return false;

            // ������Ҫ����һ����ֵ����
            if (!HasAnyValueCondition())
                return false;

            return true;
        }

        /// <summary>
        /// ����Ƿ��������κμ�ֵ����
        /// </summary>
        private bool HasAnyValueCondition()
        {
            return MinEngagementScore > 0 ||
                   MinLoyaltyScore > 0 ||
                   MinMonetaryScore > 0 ||
                   MinPotentialScore > 0 ||
                   MinOverallScore > 0 ||
                   MinEstimatedLTV > 0 ||
                   TargetSpendingLevels.Any() ||
                   TargetValueTiers.Any() ||
                   MinConversionProbability > 0 ||
                   HighValueUsersOnly ||
                   ActiveUsersOnly ||
                   LoyalUsersOnly;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var conditions = new List<string>();

            if (MinOverallScore > 0) conditions.Add($"Overall��{MinOverallScore}");
            if (MinEstimatedLTV > 0) conditions.Add($"LTV��{MinEstimatedLTV:C}");
            if (TargetValueTiers.Any()) conditions.Add($"ValueTiers:{TargetValueTiers.Count}");
            if (TargetSpendingLevels.Any()) conditions.Add($"SpendingLevels:{TargetSpendingLevels.Count}");
            if (HighValueUsersOnly) conditions.Add("HighValueOnly");
            if (ActiveUsersOnly) conditions.Add("ActiveOnly");
            if (LoyalUsersOnly) conditions.Add("LoyalOnly");

            var details = string.Join(", ", conditions);
            return $"{summary} - {details}";
        }
    }
}