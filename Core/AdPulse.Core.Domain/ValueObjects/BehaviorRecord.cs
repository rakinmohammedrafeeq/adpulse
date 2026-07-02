using AdPulse.Core.Domain.Common;

namespace AdPulse.Core.Domain.ValueObjects
{
    /// <summary>
    /// ��Ϊ��¼ֵ����
    /// ��ʾ�����û���Ϊ��¼�����ڴ洢��UserBehavior��������
    /// </summary>
    public class BehaviorRecord : ValueObject
    {
        /// <summary>
        /// ��Ϊ���ͣ��磺Click, View, Purchase�ȣ�
        /// </summary>
        public string BehaviorType { get; private set; }

        /// <summary>
        /// ��Ϊֵ������
        /// </summary>
        public string BehaviorValue { get; private set; }

        /// <summary>
        /// ��Ϊ����ʱ���
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// ��ΪƵ��
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        /// ��ΪȨ��
        /// </summary>
        public decimal Weight { get; private set; }

        /// <summary>
        /// ��Ϊ��������Ϣ��JSON��ʽ��
        /// </summary>
        public string? Context { get; private set; }

        /// <summary>
        /// ˽�й��캯��
        /// </summary>
        private BehaviorRecord()
        {
            BehaviorType = string.Empty;
            BehaviorValue = string.Empty;
            Timestamp = DateTime.UtcNow;
            Frequency = 1;
            Weight = 1.0m;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public BehaviorRecord(
            string behaviorType,
            string behaviorValue,
            DateTime? timestamp = null,
            int frequency = 1,
            decimal weight = 1.0m,
            string? context = null)
        {
            ValidateInput(behaviorType, behaviorValue, frequency, weight);

            BehaviorType = behaviorType;
            BehaviorValue = behaviorValue;
            Timestamp = timestamp ?? DateTime.UtcNow;
            Frequency = frequency;
            Weight = weight;
            Context = context;
        }

        /// <summary>
        /// ������Ϊ��¼
        /// </summary>
        public static BehaviorRecord Create(
            string behaviorType,
            string behaviorValue,
            DateTime? timestamp = null,
            int frequency = 1,
            decimal weight = 1.0m,
            string? context = null)
        {
            return new BehaviorRecord(behaviorType, behaviorValue, timestamp, frequency, weight, context);
        }

        /// <summary>
        /// ���������Ϊ��¼
        /// </summary>
        public static BehaviorRecord CreateClick(
            string target,
            DateTime? timestamp = null,
            string? context = null)
        {
            return new BehaviorRecord("Click", target, timestamp, 1, 1.0m, context);
        }

        /// <summary>
        /// ���������Ϊ��¼
        /// </summary>
        public static BehaviorRecord CreateView(
            string content,
            DateTime? timestamp = null,
            decimal duration = 0.0m,
            string? context = null)
        {
            return new BehaviorRecord("View", content, timestamp, 1, Math.Max(1.0m, duration), context);
        }

        /// <summary>
        /// ����������Ϊ��¼
        /// </summary>
        public static BehaviorRecord CreatePurchase(
            string product,
            DateTime? timestamp = null,
            decimal amount = 0.0m,
            string? context = null)
        {
            return new BehaviorRecord("Purchase", product, timestamp, 1, Math.Max(1.0m, amount), context);
        }

        /// <summary>
        /// ����������Ϊ��¼
        /// </summary>
        public static BehaviorRecord CreateSearch(
            string query,
            DateTime? timestamp = null,
            string? context = null)
        {
            return new BehaviorRecord("Search", query, timestamp, 1, 1.0m, context);
        }

        /// <summary>
        /// �Ƿ�Ϊָ�����͵���Ϊ
        /// </summary>
        public bool IsOfType(string behaviorType)
        {
            return string.Equals(BehaviorType, behaviorType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ��ȡ��Ϊ�����䣨�������ڵ�ʱ�䣩
        /// </summary>
        public TimeSpan GetAge()
        {
            return DateTime.UtcNow - Timestamp;
        }

        /// <summary>
        /// �Ƿ�Ϊ�������Ϊ��ָ��ʱ����ڣ�
        /// </summary>
        public bool IsRecent(TimeSpan timeSpan)
        {
            return GetAge() <= timeSpan;
        }

        /// <summary>
        /// ��ȡ��Ȩ������Ƶ�� * Ȩ�أ�
        /// </summary>
        public decimal GetWeightedScore()
        {
            return Frequency * Weight;
        }

        /// <summary>
        /// ��֤�������
        /// </summary>
        private static void ValidateInput(string behaviorType, string behaviorValue, int frequency, decimal weight)
        {
            if (string.IsNullOrWhiteSpace(behaviorType))
                throw new ArgumentException("��Ϊ���Ͳ���Ϊ��", nameof(behaviorType));

            if (string.IsNullOrWhiteSpace(behaviorValue))
                throw new ArgumentException("��Ϊֵ����Ϊ��", nameof(behaviorValue));

            if (frequency < 1)
                throw new ArgumentException("Ƶ�α������0", nameof(frequency));

            if (weight < 0)
                throw new ArgumentException("Ȩ�ز���Ϊ����", nameof(weight));
        }

        /// <summary>
        /// ��ȡ����ԱȽ����
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BehaviorType;
            yield return BehaviorValue;
            yield return Timestamp;
            yield return Frequency;
            yield return Weight;
            yield return Context ?? string.Empty;
        }

        /// <summary>
        /// ��ȡ�ַ�����ʾ
        /// </summary>
        public override string ToString()
        {
            var contextInfo = !string.IsNullOrEmpty(Context) ? $" ({Context})" : "";
            return $"{BehaviorType}: {BehaviorValue} x{Frequency} @{Timestamp:yyyy-MM-dd HH:mm:ss}{contextInfo}";
        }
    }
}