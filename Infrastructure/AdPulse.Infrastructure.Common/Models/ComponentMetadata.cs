namespace AdPulse.Infrastructure.Common.Models;

/// <summary>
/// ���Ԫ���ݣ�������������ĸ�����Ϣ
/// </summary>
public class ComponentMetadata
{
    /// <summary>
    /// Ԫ���ݼ�ֵ��
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// ��������Ľӿ������б�
    /// </summary>
    public List<Type> Dependencies { get; set; } = new();
    
    /// <summary>
    /// ����ṩ�ķ���ӿ��б�
    /// </summary>
    public List<Type> ProvidedServices { get; set; } = new();
    
    /// <summary>
    /// �����ǩ
    /// </summary>
    public HashSet<string> Tags { get; set; } = new();
    
    /// <summary>
    /// ����汾
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// �������
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// �������
    /// </summary>
    public string Author { get; set; } = "";
    
    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}