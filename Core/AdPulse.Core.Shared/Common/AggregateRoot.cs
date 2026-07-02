namespace AdPulse.Core.Domain.Common;

/// <summary>
/// �ۺϸ�����
/// </summary>
public abstract class AggregateRoot : EntityBase
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// ��ȡ�����¼��б�
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// ��������¼�
    /// </summary>
    /// <param name="eventItem">�����¼�</param>
    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    /// <summary>
    /// �Ƴ������¼�
    /// </summary>
    /// <param name="eventItem">�����¼�</param>
    protected void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

/// <summary>
/// �����¼��ӿ�
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// �¼�ID
    /// </summary>
    string EventId { get; }

    /// <summary>
    /// �¼�����ʱ��
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// �¼�����
    /// </summary>
    string EventType { get; }
}