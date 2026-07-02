using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// ������������ʵ����
    /// �ṩITargetingCriteria�ӿڵı�׼ʵ��
    /// </summary>
    public abstract class TargetingCriteriaBase : ValueObject, ITargetingCriteria
    {
        private readonly List<TargetingRule> _rules;

        /// <summary>
        /// ��������
        /// </summary>
        public abstract string CriteriaName { get; }

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public abstract string CriteriaType { get; }

        /// <summary>
        /// ������򼯺ϣ�ֻ����
        /// </summary>
        public IReadOnlyList<TargetingRule> Rules => _rules.AsReadOnly();

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public decimal Weight { get; protected set; } = 1.0m;

        /// <summary>
        /// �Ƿ����ø�����
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// ����������ʱ��
        /// </summary>
        public DateTime UpdatedAt { get; protected set; }

        public Guid CriteriaId { get; protected set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="rules">���򼯺�</param>
        /// <param name="weight">Ȩ��</param>
        /// <param name="isEnabled">�Ƿ�����</param>
        protected TargetingCriteriaBase(
            IEnumerable<TargetingRule>? rules = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            _rules = rules?.ToList() ?? new List<TargetingRule>();
            Weight = weight;
            IsEnabled = isEnabled;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            CriteriaId = Guid.NewGuid(); // �Զ�����ΨһID

            ValidateWeight(weight);
        }

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        public virtual T? GetRule<T>(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return default;

            var rule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (rule == null)
                return default;

            return rule.GetValue<T>();
        }

        /// <summary>
        /// ��ȡָ�����͵Ĺ���ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="ruleKey">�����</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        public virtual T GetRule<T>(string ruleKey, T defaultValue)
        {
            var result = GetRule<T>(ruleKey);
            // �����������ͣ�result ����Ϊ null������ֵ���ͣ���ö�٣���������ʧ��ͨ��Ϊ default(T)
            if (result is null)
                return defaultValue;

            if (EqualityComparer<T>.Default.Equals(result, default!))
                return defaultValue;

            return result;
        }

        /// <summary>
        /// ��ȡָ���Ĺ������
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�����������������򷵻�null</returns>
        public virtual TargetingRule? GetRuleObject(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return null;

            return _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
        }

        /// <summary>
        /// ����Ƿ����ָ���Ĺ���
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�Ƿ�����ù���</returns>
        public virtual bool HasRule(string ruleKey)
        {
            return !string.IsNullOrEmpty(ruleKey) && _rules.Any(r => r.RuleKey == ruleKey);
        }

        /// <summary>
        /// ��ȡ���й����
        /// </summary>
        /// <returns>���������</returns>
        public virtual IReadOnlyCollection<string> GetRuleKeys()
        {
            return _rules.Select(r => r.RuleKey).ToList().AsReadOnly();
        }

        /// <summary>
        /// ���ݷ����ȡ����
        /// </summary>
        /// <param name="category">�������</param>
        /// <returns>ָ������Ĺ��򼯺�</returns>
        public virtual IReadOnlyList<TargetingRule> GetRulesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new List<TargetingRule>().AsReadOnly();

            return _rules.Where(r => r.Category == category).ToList().AsReadOnly();
        }

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <returns>������򼯺�</returns>
        public virtual IReadOnlyList<TargetingRule> GetRequiredRules()
        {
            return _rules.Where(r => r.IsRequired).ToList().AsReadOnly();
        }

        /// <summary>
        /// ��֤�������õ���Ч��
        /// </summary>
        /// <returns>��֤���</returns>
        public virtual bool IsValid()
        {
            // ������֤��Ȩ�ر���Ϊ�Ǹ���
            if (Weight < 0)
                return false;

            // ���������д�˷�������ض�����֤�߼�
            return ValidateSpecificRules();
        }

        /// <summary>
        /// ��ȡ����������ժҪ��Ϣ
        /// </summary>
        /// <returns>����ժҪ</returns>
        public virtual string GetConfigurationSummary()
        {
            var ruleCount = _rules.Count;
            var enabledStatus = IsEnabled ? "Enabled" : "Disabled";
            return $"{CriteriaType} - Rules: {ruleCount}, Weight: {Weight:F2}, Status: {enabledStatus}";
        }

        /// <summary>
        /// ��ӻ���¹���
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <param name="ruleValue">����ֵ</param>
        protected virtual void SetRule(string ruleKey, object ruleValue)
        {
            if (string.IsNullOrEmpty(ruleKey))
                throw new ArgumentException("���������Ϊ��", nameof(ruleKey));

            var existingRule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (existingRule != null)
            {
                _rules.Remove(existingRule);
            }

            var newRule = new TargetingRule(ruleKey, ruleValue?.ToString() ?? string.Empty);
            _rules.Add(newRule);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="ruleKey">�����</param>
        /// <returns>�Ƿ�ɹ��Ƴ�</returns>
        protected virtual bool RemoveRule(string ruleKey)
        {
            if (string.IsNullOrEmpty(ruleKey))
                return false;

            var existingRule = _rules.FirstOrDefault(r => r.RuleKey == ruleKey);
            if (existingRule != null)
            {
                _rules.Remove(existingRule);
                UpdatedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        /// <param name="newWeight">��Ȩ��</param>
        protected virtual void UpdateWeight(decimal newWeight)
        {
            ValidateWeight(newWeight);
            Weight = newWeight;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// ��������״̬
        /// </summary>
        /// <param name="enabled">�Ƿ�����</param>
        protected virtual void UpdateEnabled(bool enabled)
        {
            IsEnabled = enabled;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// ��֤�ض��������Ч��
        /// ���������д�˷���ʵ���ض�����֤�߼�
        /// </summary>
        /// <returns>��֤���</returns>
        protected abstract bool ValidateSpecificRules();

        /// <summary>
        /// ��ȡ����ԱȽϵ����
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CriteriaType;
            yield return Weight;
            yield return IsEnabled;

            // ��������Ĺ��򼯺�
            foreach (var rule in _rules.OrderBy(r => r.RuleKey))
            {
                yield return rule.RuleKey;
                yield return rule.RuleValue ?? string.Empty;
            }
        }

        /// <summary>
        /// ��֤Ȩ��ֵ
        /// </summary>
        /// <param name="weight">Ȩ��ֵ</param>
        private static void ValidateWeight(decimal weight)
        {
            if (weight < 0)
                throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
        }
    }
}