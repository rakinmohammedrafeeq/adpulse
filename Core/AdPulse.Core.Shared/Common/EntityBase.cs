using AdPulse.Core.Shared.Enums;

namespace AdPulse.Core.Domain.Common;

/// <summary>
/// ʵ�����
/// ��������ʵ��Ļ����࣬�ṩ��ʶ��ʱ�������ɾ����ͨ�ù���
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// ʵ��Ψһ��ʶ - ʹ��Guid֧�ָ߲�������
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <summary>
    /// ����ʱ��
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// ����޸�ʱ��
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// ������
    /// </summary>
    public string CreatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// ������
    /// </summary>
    public string UpdatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// �Ƿ���ɾ������ɾ����
    /// </summary>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// ��������޸�ʱ��
    /// </summary>
    protected virtual void UpdateLastModifiedTime()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ��ɾ��
    /// </summary>
    public virtual void Delete()
    {
        IsDeleted = true;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// �ָ�ɾ��
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        UpdateLastModifiedTime();
    }

    /// <summary>
    /// ʵ����֤
    /// </summary>
    /// <returns>��֤�Ƿ�ͨ��</returns>
    public virtual bool ValidateEntity()
    {
        return Id != Guid.Empty;
    }

    /// <summary>
    /// ����ԱȽ�
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other || GetType() != other.GetType())
            return false;

        return Id == other.Id;
    }

    /// <summary>
    /// ��ȡ��ϣ��
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// ����Բ�����
    /// </summary>
    public static bool operator ==(EntityBase? left, EntityBase? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// �����Բ�����
    /// </summary>
    public static bool operator !=(EntityBase? left, EntityBase? right)
    {
        return !Equals(left, right);
    }
}