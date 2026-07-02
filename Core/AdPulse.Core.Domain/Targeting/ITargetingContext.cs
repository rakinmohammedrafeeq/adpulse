using AdPulse.Core.Domain.ValueObjects;

namespace AdPulse.Core.Domain.Targeting
{
    /// <summary>
    /// ���������ĳ���ӿ�
    /// ����ͳһ�����������ݷ��ʹ淶��֧�ֶ������͵Ķ�����Ϣ�洢�ͼ���
    /// </summary>
    public interface ITargetingContext
    {
        /// <summary>
        /// ����������
        /// ���ڱ�ʶ�����������ĵ��Ѻ�����
        /// </summary>
        string ContextName { get; }

        /// <summary>
        /// ���������ͱ�ʶ
        /// ���磺AdRequest��UserProfile��DeviceInfo��
        /// </summary>
        string ContextType { get; }

        /// <summary>
        /// ���������Լ��� - ���ϵ�������
        /// �洢���ֶ�����ص�������Ϣ�����ԭ�е�Properties�ֵ�
        /// </summary>
        IReadOnlyList<ContextProperty> Properties { get; }

        /// <summary>
        /// �����Ĵ���ʱ���
        /// ���ڻ�������������Ч�Լ��
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// �����ĵ�Ψһ��ʶ
        /// ���ڻ�������ɺ͵���׷��
        /// </summary>
        Guid ContextId { get; }

        /// <summary>
        /// ������������Դ
        /// ��ʶ���ݵ���Դϵͳ�����������������������
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// ��ȡָ����������ʵ��
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ʵ�壬����������򷵻�null</returns>
        ContextProperty? GetProperty(string propertyKey);

        /// <summary>
        /// ��ȡָ�����͵�����ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ������������򷵻�Ĭ��ֵ</returns>
        T? GetPropertyValue<T>(string propertyKey);

        /// <summary>
        /// ��ȡָ�����͵�����ֵ������������򷵻�Ĭ��ֵ
        /// </summary>
        /// <typeparam name="T">����ֵ����</typeparam>
        /// <param name="propertyKey">���Լ�</param>
        /// <param name="defaultValue">Ĭ��ֵ</param>
        /// <returns>����ֵ��Ĭ��ֵ</returns>
        T GetPropertyValue<T>(string propertyKey, T defaultValue);

        /// <summary>
        /// ��ȡ����ֵ���ַ�����ʾ
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>����ֵ���ַ�����ʾ������������򷵻ؿ��ַ���</returns>
        string GetPropertyAsString(string propertyKey);

        /// <summary>
        /// ����Ƿ����ָ��������
        /// </summary>
        /// <param name="propertyKey">���Լ�</param>
        /// <returns>�Ƿ����������</returns>
        bool HasProperty(string propertyKey);

        /// <summary>
        /// ��ȡ�������Լ�
        /// </summary>
        /// <returns>���Լ�����</returns>
        IReadOnlyCollection<string> GetPropertyKeys();

        /// <summary>
        /// ��ȡָ�����������
        /// </summary>
        /// <param name="category">���Է���</param>
        /// <returns>���Լ���</returns>
        IReadOnlyList<ContextProperty> GetPropertiesByCategory(string category);

        /// <summary>
        /// ��ȡδ���ڵ�����
        /// </summary>
        /// <returns>δ���ڵ����Լ���</returns>
        IReadOnlyList<ContextProperty> GetActiveProperties();

        /// <summary>
        /// ��������������Ƿ���Ч
        /// ����ʱ��������������Խ�����֤
        /// </summary>
        /// <returns>�Ƿ���Ч</returns>
        bool IsValid();

        /// <summary>
        /// ��������������Ƿ��ѹ���
        /// </summary>
        /// <param name="maxAge">�����Ч��</param>
        /// <returns>�Ƿ��ѹ���</returns>
        bool IsExpired(TimeSpan maxAge);

        /// <summary>
        /// ��ȡ�����ĵ�Ԫ������Ϣ
        /// ����������Դ������ʱ�䡢������������Ϣ
        /// </summary>
        /// <returns>Ԫ�������Լ���</returns>
        IReadOnlyList<ContextProperty> GetMetadata();

        /// <summary>
        /// ��ȡ�����ĵĵ�����Ϣ
        /// ���������Ų����־��¼
        /// </summary>
        /// <returns>������Ϣ�ַ���</returns>
        string GetDebugInfo();

        /// <summary>
        /// ���������ĵ�����������
        /// ֻ����ָ�������Լ������������Ż�
        /// </summary>
        /// <param name="includeKeys">Ҫ���������Լ�</param>
        /// <returns>�����������ĸ���</returns>
        ITargetingContext CreateLightweightCopy(IEnumerable<string> includeKeys);

        /// <summary>
        /// ���������ĵķ��ั��
        /// ֻ����ָ����������ԣ����ڶ�������Ż�
        /// </summary>
        /// <param name="categories">Ҫ���������Է���</param>
        /// <returns>���������ĸ���</returns>
        ITargetingContext CreateCategorizedCopy(IEnumerable<string> categories);

        /// <summary>
        /// �ϲ���һ�������ĵ�����
        /// ������϶������Դ����������Ϣ
        /// </summary>
        /// <param name="other">Ҫ�ϲ���������</param>
        /// <param name="overwriteExisting">�Ƿ񸲸��Ѵ��ڵ�����</param>
        /// <returns>�ϲ������������</returns>
        ITargetingContext Merge(ITargetingContext other, bool overwriteExisting = false);
    }
}