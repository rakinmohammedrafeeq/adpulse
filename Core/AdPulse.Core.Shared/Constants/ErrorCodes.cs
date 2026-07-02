namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// �����볣��
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// ͨ�ô�����
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// δ֪����
        /// </summary>
        public const string UnknownError = "E0001";

        /// <summary>
        /// ������Ч
        /// </summary>
        public const string InvalidParameter = "E0002";

        /// <summary>
        /// ���ݲ�����
        /// </summary>
        public const string DataNotFound = "E0003";

        /// <summary>
        /// �����ظ�
        /// </summary>
        public const string DataDuplicate = "E0004";

        /// <summary>
        /// Ȩ�޲���
        /// </summary>
        public const string InsufficientPermission = "E0005";

        /// <summary>
        /// ����ʱ
        /// </summary>
        public const string RequestTimeout = "E0006";

        /// <summary>
        /// ϵͳ��æ
        /// </summary>
        public const string SystemBusy = "E0007";

        /// <summary>
        /// �ڲ�����
        /// </summary>
        public const string InternalError = "E0008";

        /// <summary>
        /// ������ȡ��
        /// </summary>
        public const string OperationCancelled = "E0009";
    }

    /// <summary>
    /// �����ش�����
    /// </summary>
    public static class Advertisement
    {
        /// <summary>
        /// ��治����
        /// </summary>
        public const string AdNotFound = "E1001";

        /// <summary>
        /// ����ѹ���
        /// </summary>
        public const string AdExpired = "E1002";

        /// <summary>
        /// ���δ���ͨ��
        /// </summary>
        public const string AdNotApproved = "E1003";

        /// <summary>
        /// ���Ԥ�㲻��
        /// </summary>
        public const string InsufficientBudget = "E1004";

        /// <summary>
        /// ��涨��ƥ��
        /// </summary>
        public const string TargetingMismatch = "E1005";

        /// <summary>
        /// ���Ƶ�γ���
        /// </summary>
        public const string FrequencyCapExceeded = "E1006";

        /// <summary>
        /// ����ز���Ч
        /// </summary>
        public const string InvalidCreative = "E1007";
    }

    /// <summary>
    /// ������ش�����
    /// </summary>
    public static class Bidding
    {
        /// <summary>
        /// ����������Ч
        /// </summary>
        public const string InvalidBidRequest = "E2001";

        /// <summary>
        /// ���ۼ۸����
        /// </summary>
        public const string BidPriceTooLow = "E2002";

        /// <summary>
        /// ���ۼ۸����
        /// </summary>
        public const string BidPriceTooHigh = "E2003";

        /// <summary>
        /// ���۳�ʱ
        /// </summary>
        public const string BidTimeout = "E2004";

        /// <summary>
        /// ����Ч����
        /// </summary>
        public const string NoBidAvailable = "E2005";

        /// <summary>
        /// ����ʧ��
        /// </summary>
        public const string BidFailed = "E2006";
    }

    /// <summary>
    /// �û���ش�����
    /// </summary>
    public static class User
    {
        /// <summary>
        /// �û�������
        /// </summary>
        public const string UserNotFound = "E3001";

        /// <summary>
        /// �û��Ѵ���
        /// </summary>
        public const string UserAlreadyExists = "E3002";

        /// <summary>
        /// �û�δ����
        /// </summary>
        public const string UserNotActivated = "E3003";

        /// <summary>
        /// �û��ѱ�����
        /// </summary>
        public const string UserDisabled = "E3004";

        /// <summary>
        /// �û���֤ʧ��
        /// </summary>
        public const string AuthenticationFailed = "E3005";

        /// <summary>
        /// �û���Ȩʧ��
        /// </summary>
        public const string AuthorizationFailed = "E3006";
    }

    /// <summary>
    /// ���ݷ��ʴ�����
    /// </summary>
    public static class DataAccess
    {
        /// <summary>
        /// ���ݿ�����ʧ��
        /// </summary>
        public const string DatabaseConnectionFailed = "E4001";

        /// <summary>
        /// ���ݿ��ѯʧ��
        /// </summary>
        public const string DatabaseQueryFailed = "E4002";

        /// <summary>
        /// ���ݿ����ʧ��
        /// </summary>
        public const string DatabaseUpdateFailed = "E4003";

        /// <summary>
        /// �������ʧ��
        /// </summary>
        public const string CacheAccessFailed = "E4004";

        /// <summary>
        /// �������л�ʧ��
        /// </summary>
        public const string DataSerializationFailed = "E4005";

        /// <summary>
        /// ���ݷ����л�ʧ��
        /// </summary>
        public const string DataDeserializationFailed = "E4006";

        /// <summary>
        /// ������֤ʧ��
        /// </summary>
        public const string DataValidationFailed = "E4007";

        /// <summary>
        /// ����ת��ʧ��
        /// </summary>
        public const string DataTransformationFailed = "E4008";

        /// <summary>
        /// ����Դ������
        /// </summary>
        public const string DataSourceUnavailable = "E4009";
    }

    /// <summary>
    /// �ⲿ���������
    /// </summary>
    public static class ExternalService
    {
        /// <summary>
        /// �ⲿ���񲻿���
        /// </summary>
        public const string ServiceUnavailable = "E5001";

        /// <summary>
        /// �ⲿ������Ӧ��ʱ
        /// </summary>
        public const string ServiceTimeout = "E5002";

        /// <summary>
        /// �ⲿ������Ӧ��ʽ����
        /// </summary>
        public const string InvalidServiceResponse = "E5003";

        /// <summary>
        /// �ⲿ������֤ʧ��
        /// </summary>
        public const string ServiceAuthenticationFailed = "E5004";

        /// <summary>
        /// �ⲿ��������
        /// </summary>
        public const string ServiceRateLimited = "E5005";
    }

    /// <summary>
    /// ������������
    /// </summary>
    public static class AdEngine
    {
        /// <summary>
        /// ������ش�����
        /// </summary>
        public static class Strategy
        {
            /// <summary>
            /// ����δ�ҵ�
            /// </summary>
            public const string StrategyNotFound = "E6001";

            /// <summary>
            /// ����ִ��ʧ��
            /// </summary>
            public const string StrategyExecutionFailed = "E6002";

            /// <summary>
            /// ����ִ�г�ʱ
            /// </summary>
            public const string StrategyTimeout = "E6003";

            /// <summary>
            /// ����������Ч
            /// </summary>
            public const string StrategyConfigurationInvalid = "E6004";

            /// <summary>
            /// ��������ȱʧ
            /// </summary>
            public const string StrategyDependencyMissing = "E6005";

            /// <summary>
            /// ������֤ʧ��
            /// </summary>
            public const string StrategyValidationFailed = "E6006";
        }

        /// <summary>
        /// �ص���ش�����
        /// </summary>
        public static class Callback
        {
            /// <summary>
            /// �ص�δ�ҵ�
            /// </summary>
            public const string CallbackNotFound = "E6101";

            /// <summary>
            /// �ص�ִ��ʧ��
            /// </summary>
            public const string CallbackExecutionFailed = "E6102";

            /// <summary>
            /// �ص�ִ�г�ʱ
            /// </summary>
            public const string CallbackTimeout = "E6103";

            /// <summary>
            /// �ص�������Ч
            /// </summary>
            public const string CallbackConfigurationInvalid = "E6104";

            /// <summary>
            /// �ص��������ʧ��
            /// </summary>
            public const string CallbackHealthCheckFailed = "E6105";
        }

        /// <summary>
        /// ������ش�����
        /// </summary>
        public static class Configuration
        {
            /// <summary>
            /// ����δ�ҵ�
            /// </summary>
            public const string ConfigurationNotFound = "E6201";

            /// <summary>
            /// ������Ч
            /// </summary>
            public const string ConfigurationInvalid = "E6202";

            /// <summary>
            /// ���ü���ʧ��
            /// </summary>
            public const string ConfigurationLoadFailed = "E6203";
        }

        /// <summary>
        /// ��Դ��ش�����
        /// </summary>
        public static class Resource
        {
            /// <summary>
            /// ��Դ�ľ�
            /// </summary>
            public const string ResourceExhausted = "E6301";

            /// <summary>
            /// ��Դ������
            /// </summary>
            public const string ResourceUnavailable = "E6302";

            /// <summary>
            /// ��Դ���Ƴ���
            /// </summary>
            public const string ResourceLimitExceeded = "E6303";
        }

        /// <summary>
        /// ҵ�������ش�����
        /// </summary>
        public static class BusinessRule
        {
            /// <summary>
            /// ҵ�����Υ��
            /// </summary>
            public const string BusinessRuleViolation = "E6401";

            /// <summary>
            /// ҵ���������ʧ��
            /// </summary>
            public const string BusinessRuleEvaluationFailed = "E6402";
        }
    }
}