using AdPulse.Infrastructure.Common.Models;

namespace AdPulse.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// ������������
/// ������ʽ���������������ϵ
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependencyAttribute : Attribute
{
    /// <summary>
    /// �����ķ�������
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// �Ƿ�Ϊ��ѡ����
    /// </summary>
    public bool IsOptional { get; set; } = false;

    /// <summary>
    /// ��������������Ҫ��
    /// </summary>
    public ServiceLifetime? RequiredLifetime { get; set; }

    /// <summary>
    /// ��������
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// ��ʼ��������������
    /// </summary>
    /// <param name="serviceType">�����ķ�������</param>
    public DependencyAttribute(Type serviceType)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    }
}