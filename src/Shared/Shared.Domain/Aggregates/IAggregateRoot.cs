using RoboForge.Shared.Domain.Events;

namespace RoboForge.Shared.Domain.Aggregates;

/// <summary>
/// The domain events of any aggregate, whatever its ID type, so persistence can collect them from
/// every tracked aggregate when saving.
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
