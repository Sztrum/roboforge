using Mediator;

namespace RoboForge.Shared.IntegrationEvents;

/// <summary>
/// A fact one module publishes for the others. Unlike a domain event it is a stable contract of plain
/// values: modules never see each other's domain types (docs/PROJECT.md, "Domain events and integration").
/// It travels through the publishing module's outbox, so it is delivered only after its transaction commits.
/// </summary>
public abstract record IntegrationEvent : INotification
{
    public Guid Id { get; init; } = Guid.CreateVersion7();

    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
}
