using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Catalog.Domain.Parts.Events;

public sealed record PartDiscontinuedEvent(PartId PartId) : IDomainEvent;
