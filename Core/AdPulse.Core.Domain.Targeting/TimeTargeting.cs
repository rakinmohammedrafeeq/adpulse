using AdPulse.Core.Domain.Targeting;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// ʱ�䶨������
    /// ʵ�ֻ���ʱ�䷶Χ�Ķ����������
    /// </summary>
    public class TimeTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "ʱ�䶨��";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Time";

        /// <summary>
        /// ���ڼ��б�
        /// </summary>
        public IReadOnlyList<DayOfWeek> Days => GetRule<List<DayOfWeek>>("Days") ?? new List<DayOfWeek>();

        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        public TimeOnly StartTime => GetRule("StartTime", TimeOnly.MinValue);

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public TimeOnly EndTime => GetRule("EndTime", TimeOnly.MaxValue);

        /// <summary>
        /// ʱ��
        /// </summary>
        public string TimeZoneId => GetRule("TimeZoneId", "UTC");

        /// <summary>
        /// ���캯��
        /// </summary>
        public TimeTargeting(
            IList<DayOfWeek>? days = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null,
            string timeZoneId = "UTC",
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(days, startTime, endTime, timeZoneId), weight, isEnabled)
        {
        }

        /// <summary>
        /// ���������ֵ�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<DayOfWeek>? days,
            TimeOnly? startTime,
            TimeOnly? endTime,
            string timeZoneId)
        {
            var rules = new List<TargetingRule>();

            // ��������б�
            var daysList = days?.ToList() ?? Enum.GetValues<DayOfWeek>().ToList();
            var daysRule = new TargetingRule("Days", string.Empty, "Json").WithValue(daysList);
            rules.Add(daysRule);

            // ��ӿ�ʼʱ��
            var startTimeValue = startTime ?? TimeOnly.MinValue;
            var startTimeRule = new TargetingRule("StartTime", string.Empty, "TimeOnly").WithValue(startTimeValue);
            rules.Add(startTimeRule);

            // ��ӽ���ʱ��
            var endTimeValue = endTime ?? TimeOnly.MaxValue;
            var endTimeRule = new TargetingRule("EndTime", string.Empty, "TimeOnly").WithValue(endTimeValue);
            rules.Add(endTimeRule);

            // ���ʱ��ID
            var timeZoneRule = new TargetingRule("TimeZoneId", string.Empty, "String").WithValue(timeZoneId ?? "UTC");
            rules.Add(timeZoneRule);

            return rules;
        }

        /// <summary>
        /// ����ʱ�䶨������
        /// </summary>
        public static TimeTargeting Create(
            IList<DayOfWeek>? days = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null,
            string timeZoneId = "UTC",
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new TimeTargeting(days, startTime, endTime, timeZoneId, weight, isEnabled);
        }

        /// <summary>
        /// ����ʱ�䷶Χ
        /// </summary>
        /// <param name="startTime">��ʼʱ��</param>
        /// <param name="endTime">����ʱ��</param>
        public void SetTimeRange(TimeOnly startTime, TimeOnly endTime)
        {
            SetRule("StartTime", startTime);
            SetRule("EndTime", endTime);
        }

        /// <summary>
        /// �������ڼ�
        /// </summary>
        /// <param name="days">���ڼ��б�</param>
        public void SetDays(IList<DayOfWeek> days)
        {
            if (days == null)
                throw new ArgumentNullException(nameof(days));

            SetRule("Days", days.ToList());
        }

        /// <summary>
        /// ������ڼ�
        /// </summary>
        /// <param name="dayOfWeek">���ڼ�</param>
        public void AddDay(DayOfWeek dayOfWeek)
        {
            var currentList = Days.ToList();
            if (!currentList.Contains(dayOfWeek))
            {
                currentList.Add(dayOfWeek);
                SetRule("Days", currentList);
            }
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <param name="timeZoneId">ʱ����ʶ</param>
        public void SetTimeZone(string timeZoneId)
        {
            if (string.IsNullOrEmpty(timeZoneId))
                throw new ArgumentException("ʱ����ʶ����Ϊ��", nameof(timeZoneId));

            SetRule("TimeZoneId", timeZoneId);
        }

        /// <summary>
        /// ����Ƿ���ָ��ʱ�伤��
        /// </summary>
        /// <param name="dateTime">���ʱ��</param>
        /// <returns>�Ƿ񼤻�</returns>
        public bool IsActiveAt(DateTime dateTime)
        {
            // ������ڼ�
            if (Days.Any() && !Days.Contains(dateTime.DayOfWeek))
                return false;

            // ���ʱ�䷶Χ
            var time = TimeOnly.FromDateTime(dateTime);
            if (StartTime <= EndTime)
            {
                return time >= StartTime && time <= EndTime;
            }
            else
            {
                // ����ʱ�䷶Χ
                return time >= StartTime || time <= EndTime;
            }
        }

        /// <summary>
        /// ��֤ʱ�䶨���ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ��֤ʱ����ʶ
            if (string.IsNullOrWhiteSpace(TimeZoneId))
                return false;

            // ��֤������һ�챻ѡ��
            if (!Days.Any())
                return false;

            // ��֤ʱ����ʶ��ʽ������֤��
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var timeRange = StartTime == TimeOnly.MinValue && EndTime == TimeOnly.MaxValue
                ? "All Day"
                : $"{StartTime:HH:mm}-{EndTime:HH:mm}";
            var details = $"Days: {Days.Count}, Time: {timeRange}, TimeZone: {TimeZoneId}";
            return $"{summary} - {details}";
        }
    }
}