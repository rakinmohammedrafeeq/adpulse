namespace AdPulse.Core.Shared.Constants;

/// <summary>
/// ��֤������
/// </summary>
public static class ValidationConstants
{
    /// <summary>
    /// �ַ�����������
    /// </summary>
    public static class StringLength
    {
        /// <summary>
        /// ���ID��󳤶�
        /// </summary>
        public const int AdIdMaxLength = 50;

        /// <summary>
        /// �û�ID��󳤶�
        /// </summary>
        public const int UserIdMaxLength = 100;

        /// <summary>
        /// ������󳤶�
        /// </summary>
        public const int DomainMaxLength = 255;

        /// <summary>
        /// �ؼ�����󳤶�
        /// </summary>
        public const int KeywordsMaxLength = 1000;

        /// <summary>
        /// ��������󳤶�
        /// </summary>
        public const int AdTitleMaxLength = 100;

        /// <summary>
        /// ���������󳤶�
        /// </summary>
        public const int AdDescriptionMaxLength = 500;

        /// <summary>
        /// URL��󳤶�
        /// </summary>
        public const int UrlMaxLength = 2048;

        /// <summary>
        /// ý��������󳤶�
        /// </summary>
        public const int MediaNameMaxLength = 100;
        /// <summary>
        /// ��˾������󳤶�
        /// </summary>
        public const int CompanyNameMaxLength = 100;

        /// <summary>
        /// �������󳤶�
        /// </summary>
        public const int CampaignNameMaxLength = 100;
    }

    /// <summary>
    /// ��ֵ��Χ����
    /// </summary>
    public static class NumberRange
    {
        /// <summary>
        /// ��С���ۼ۸� (��)
        /// </summary>
        public const decimal MinBidPrice = 0.01m;

        /// <summary>
        /// �����ۼ۸� (Ԫ)
        /// </summary>
        public const decimal MaxBidPrice = 10000.00m;

        /// <summary>
        /// ��СԤ���� (Ԫ)
        /// </summary>
        public const decimal MinBudget = 1.00m;

        /// <summary>
        /// ���Ԥ���� (Ԫ)
        /// </summary>
        public const decimal MaxBudget = 1000000.00m;

        /// <summary>
        /// ��С����
        /// </summary>
        public const int MinAge = 13;

        /// <summary>
        /// �������
        /// </summary>
        public const int MaxAge = 100;

        /// <summary>
        /// ��С�����
        /// </summary>
        public const int MinAdWidth = 50;

        /// <summary>
        /// �������
        /// </summary>
        public const int MaxAdWidth = 4096;

        /// <summary>
        /// ��С���߶�
        /// </summary>
        public const int MinAdHeight = 50;

        /// <summary>
        /// �����߶�
        /// </summary>
        public const int MaxAdHeight = 4096;
    }

    /// <summary>
    /// ���ϴ�С����
    /// </summary>
    public static class CollectionSize
    {
        /// <summary>
        /// �������������λ����
        /// </summary>
        public const int MaxImpressionsPerRequest = 10;

        /// <summary>
        /// ���ؼ�������
        /// </summary>
        public const int MaxKeywordsCount = 100;

        /// <summary>
        /// �����������
        /// </summary>
        public const int MaxDomainsCount = 50;

        /// <summary>
        /// ����������
        /// </summary>
        public const int MaxCategoriesCount = 20;

        /// <summary>
        /// �����������
        /// </summary>
        public const int MaxAttributesCount = 50;
    }

    /// <summary>
    /// ������ʽģʽ
    /// </summary>
    public static class RegexPatterns
    {
        /// <summary>
        /// UUID��ʽ��֤
        /// </summary>
        public const string UuidPattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";

        /// <summary>
        /// ������ʽ��֤
        /// </summary>
        public const string DomainPattern = @"^[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?)*$";

        /// <summary>
        /// �����ʽ��֤
        /// </summary>
        public const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        /// <summary>
        /// IP��ַ��ʽ��֤
        /// </summary>
        public const string IpAddressPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        /// <summary>
        /// URL��ʽ��֤
        /// </summary>
        public const string UrlPattern = @"^https?://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,}(/.*)?$";
    }
}