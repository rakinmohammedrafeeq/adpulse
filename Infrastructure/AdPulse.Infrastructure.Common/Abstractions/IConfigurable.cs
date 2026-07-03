namespace AdPulse.Infrastructure.Common.Abstractions;

/// <summary>
/// �������������Ļ����ӿ�
/// ʵ�ִ˽ӿڵ�������Խ������ñ��֪ͨ��������������
/// </summary>
public interface IConfigurable
{
    /// <summary>
    /// �����÷������ʱ���ô˷��������������
    /// </summary>
    /// <param name="configuration">�µ����ö���</param>
    void Configure(object configuration);
    
    /// <summary>
    /// ��ȡ�������������
    /// </summary>
    Type ConfigurationType { get; }
}