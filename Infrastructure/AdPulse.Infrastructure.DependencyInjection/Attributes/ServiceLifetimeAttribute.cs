using AdPulse.Infrastructure.Common.Models;

namespace AdPulse.Infrastructure.DependencyInjection.Attributes;

/// <summary>
/// ����������������
/// ������ʽָ���������������
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    /// <summary>
    /// ������������
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// ��ʼ������������������
    /// </summary>
    /// <param name="lifetime">������������</param>
    public ServiceLifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}

/// <summary>
/// ������������
/// </summary>
public class SingletonAttribute : ServiceLifetimeAttribute
{
    public SingletonAttribute() : base(ServiceLifetime.Singleton) { }
}

/// <summary>
/// �������������
/// </summary>
public class ScopedAttribute : ServiceLifetimeAttribute
{
    public ScopedAttribute() : base(ServiceLifetime.Scoped) { }
}

/// <summary>
/// ˲̬��������
/// </summary>
public class TransientAttribute : ServiceLifetimeAttribute
{
    public TransientAttribute() : base(ServiceLifetime.Transient) { }
}