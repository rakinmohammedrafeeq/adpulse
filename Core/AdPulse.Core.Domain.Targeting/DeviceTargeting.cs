using AdPulse.Core.Domain.Targeting;
using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// �豸��������
    /// ʵ�ֻ����豸���ԵĶ����������
    /// </summary>
    public class DeviceTargeting : TargetingCriteriaBase
    {
        /// <summary>
        /// ��������
        /// </summary>
        public override string CriteriaName => "�豸����";

        /// <summary>
        /// �������ͱ�ʶ
        /// </summary>
        public override string CriteriaType => "Device";

        /// <summary>
        /// �豸�����б�
        /// </summary>
        public IReadOnlyList<DeviceType> DeviceTypes => GetRule<List<DeviceType>>("DeviceTypes") ?? new List<DeviceType>();

        /// <summary>
        /// ����ϵͳ�б�
        /// </summary>
        public IReadOnlyList<string> OperatingSystems => GetRule<List<string>>("OperatingSystems") ?? new List<string>();

        /// <summary>
        /// ������б�
        /// </summary>
        public IReadOnlyList<string> Browsers => GetRule<List<string>>("Browsers") ?? new List<string>();

        /// <summary>
        /// ���캯��
        /// </summary>
        public DeviceTargeting(
            IList<DeviceType>? deviceTypes = null,
            IList<string>? operatingSystems = null,
            IList<string>? browsers = null,
            decimal weight = 1.0m,
            bool isEnabled = true) : base(CreateRules(deviceTypes, operatingSystems, browsers), weight, isEnabled)
        {
        }

        /// <summary>
        /// ���������ֵ�
        /// </summary>
        private static IEnumerable<TargetingRule> CreateRules(
            IList<DeviceType>? deviceTypes,
            IList<string>? operatingSystems,
            IList<string>? browsers)
        {
            var rules = new List<TargetingRule>();

            // ����豸�����б�
            var deviceTypesList = deviceTypes?.ToList() ?? new List<DeviceType>();
            var deviceTypesRule = new TargetingRule("DeviceTypes", string.Empty, "Json").WithValue(deviceTypesList);
            rules.Add(deviceTypesRule);

            // ��Ӳ���ϵͳ�б�
            var operatingSystemsList = operatingSystems?.ToList() ?? new List<string>();
            var operatingSystemsRule = new TargetingRule("OperatingSystems", string.Empty, "Json").WithValue(operatingSystemsList);
            rules.Add(operatingSystemsRule);

            // ���������б�
            var browsersList = browsers?.ToList() ?? new List<string>();
            var browsersRule = new TargetingRule("Browsers", string.Empty, "Json").WithValue(browsersList);
            rules.Add(browsersRule);

            return rules;
        }

        /// <summary>
        /// �����豸��������
        /// </summary>
        public static DeviceTargeting Create(
            IList<DeviceType>? deviceTypes = null,
            IList<string>? operatingSystems = null,
            IList<string>? browsers = null,
            decimal weight = 1.0m,
            bool isEnabled = true)
        {
            return new DeviceTargeting(deviceTypes, operatingSystems, browsers, weight, isEnabled);
        }

        /// <summary>
        /// ����豸����
        /// </summary>
        /// <param name="deviceType">�豸����</param>
        public void AddDeviceType(DeviceType deviceType)
        {
            var currentList = DeviceTypes.ToList();
            if (!currentList.Contains(deviceType))
            {
                currentList.Add(deviceType);
                SetRule("DeviceTypes", currentList);
            }
        }

        /// <summary>
        /// ��Ӳ���ϵͳ
        /// </summary>
        /// <param name="operatingSystem">����ϵͳ</param>
        public void AddOperatingSystem(string operatingSystem)
        {
            if (string.IsNullOrEmpty(operatingSystem))
                throw new ArgumentException("����ϵͳ����Ϊ��", nameof(operatingSystem));

            var currentList = OperatingSystems.ToList();
            if (!currentList.Contains(operatingSystem))
            {
                currentList.Add(operatingSystem);
                SetRule("OperatingSystems", currentList);
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="browser">�����</param>
        public void AddBrowser(string browser)
        {
            if (string.IsNullOrEmpty(browser))
                throw new ArgumentException("���������Ϊ��", nameof(browser));

            var currentList = Browsers.ToList();
            if (!currentList.Contains(browser))
            {
                currentList.Add(browser);
                SetRule("Browsers", currentList);
            }
        }

        /// <summary>
        /// ��֤�豸�����ض��������Ч��
        /// </summary>
        protected override bool ValidateSpecificRules()
        {
            // ������Ҫ����һ���豸����
            if (!DeviceTypes.Any() && !OperatingSystems.Any() && !Browsers.Any())
                return false;

            // ��֤����ϵͳ����������Ʋ���Ϊ���ַ���
            if (OperatingSystems.Any(string.IsNullOrWhiteSpace) || Browsers.Any(string.IsNullOrWhiteSpace))
                return false;

            return true;
        }

        /// <summary>
        /// ��ȡ����ժҪ��Ϣ
        /// </summary>
        public override string GetConfigurationSummary()
        {
            var summary = base.GetConfigurationSummary();
            var details = $"DeviceTypes: {DeviceTypes.Count}, OS: {OperatingSystems.Count}, Browsers: {Browsers.Count}";
            return $"{summary} - {details}";
        }
    }
}