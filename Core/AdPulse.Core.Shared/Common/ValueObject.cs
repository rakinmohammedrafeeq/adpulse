namespace AdPulse.Core.Domain.Common;

/// <summary>
/// ֵ�������
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Ψһ��ʶ��
    /// </summary>
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    /// <summary>
    /// ��ȡ����ԱȽϵ����
    /// </summary>
    /// <returns>��������ԱȽϵ��������</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// ����ԱȽ�
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// ��ȡ��ϣ��
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// ����Բ�����
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// �����Բ�����
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// ����ֵ����ĸ���
    /// </summary>
    public ValueObject Copy()
    {
        return (ValueObject)MemberwiseClone();
    }
}