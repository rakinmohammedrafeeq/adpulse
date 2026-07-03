namespace AdPulse.Infrastructure.Common.Conventions;

/// <summary>
/// ����Լ���淶
/// </summary>
public static class NamingConventions
{
    /// <summary>
    /// ���������׺����
    /// </summary>
    public static class ComponentSuffixes
    {
        public const string Strategy = "Strategy";
        public const string Service = "Service";
        public const string Manager = "Manager";
        public const string Provider = "Provider";
        public const string CallbackProvider = "CallbackProvider";
        public const string Matcher = "Matcher";
        public const string Calculator = "Calculator";
        public const string Processor = "Processor";
        public const string Handler = "Handler";
        public const string Factory = "Factory";
        public const string Repository = "Repository";
        public const string Controller = "Controller";
    }

    /// <summary>
    /// ������������׺
    /// </summary>
    public static class ConfigurationSuffixes
    {
        public const string Options = "Options";
        public const string Settings = "Settings";
        public const string Configuration = "Configuration";
    }

    /// <summary>
    /// �ӿ�����ǰ׺
    /// </summary>
    public static class InterfacePrefixes
    {
        public const string Interface = "I";
    }

    /// <summary>
    /// ������������Ƿ�����������Լ��
    /// </summary>
    /// <param name="typeName">��������</param>
    /// <returns>�Ƿ����Լ��</returns>
    public static bool IsComponentType(string typeName)
    {
        return typeName.EndsWith(ComponentSuffixes.Strategy) ||
               typeName.EndsWith(ComponentSuffixes.Service) ||
               typeName.EndsWith(ComponentSuffixes.Manager) ||
               typeName.EndsWith(ComponentSuffixes.Provider) ||
               typeName.EndsWith(ComponentSuffixes.CallbackProvider) ||
               typeName.EndsWith(ComponentSuffixes.Matcher) ||
               typeName.EndsWith(ComponentSuffixes.Calculator) ||
               typeName.EndsWith(ComponentSuffixes.Processor) ||
               typeName.EndsWith(ComponentSuffixes.Handler) ||
               typeName.EndsWith(ComponentSuffixes.Factory) ||
               typeName.EndsWith(ComponentSuffixes.Repository) ||
               typeName.EndsWith(ComponentSuffixes.Controller);
    }

    /// <summary>
    /// ������������Ƿ��������������Լ��
    /// </summary>
    /// <param name="typeName">��������</param>
    /// <returns>�Ƿ����Լ��</returns>
    public static bool IsConfigurationType(string typeName)
    {
        return typeName.EndsWith(ConfigurationSuffixes.Options) ||
               typeName.EndsWith(ConfigurationSuffixes.Settings) ||
               typeName.EndsWith(ConfigurationSuffixes.Configuration);
    }

    /// <summary>
    /// ��������������ƻ�ȡ��Ӧ�Ľӿ�����
    /// </summary>
    /// <param name="componentTypeName">�����������</param>
    /// <returns>�ӿ�����</returns>
    public static string GetInterfaceName(string componentTypeName)
    {
        return $"{InterfacePrefixes.Interface}{componentTypeName}";
    }

    /// <summary>
    /// �������������ƻ�ȡ���ý�����
    /// </summary>
    /// <param name="configurationTypeName">����������</param>
    /// <returns>���ý�����</returns>
    public static string GetConfigurationSectionName(string configurationTypeName)
    {
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Options))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Options.Length);
        
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Settings))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Settings.Length);
        
        if (configurationTypeName.EndsWith(ConfigurationSuffixes.Configuration))
            return configurationTypeName.Substring(0, configurationTypeName.Length - ConfigurationSuffixes.Configuration.Length);
        
        return configurationTypeName;
    }
}