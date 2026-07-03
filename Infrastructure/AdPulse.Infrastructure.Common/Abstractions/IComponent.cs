namespace AdPulse.Infrastructure.Common.Abstractions;

/// <summary>
/// ��������Ļ����ӿ�
/// ����ҵ�������Ӧ�ü̳д˽ӿڻ����ӽӿ�
/// </summary>
public interface IComponent
{
    /// <summary>
    /// �������
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// ����Ƿ��ѳ�ʼ��
    /// </summary>
    bool IsInitialized { get; }
    
    /// <summary>
    /// ��ʼ�����
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// �ͷ������Դ
    /// </summary>
    Task DisposeAsync();
}