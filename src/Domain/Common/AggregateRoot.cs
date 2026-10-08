namespace RoboForge.Domain.Common;

/// <summary>
/// The only entry point for changing the data of an aggregate. Collects the domain events
/// raised by its business methods until they are persisted.
/// </summary>
public abstract class AggregateRoot<TId>
    where TId : struct
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    public TId Id { get; private init; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
