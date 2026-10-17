using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.Parts.Events;

public sealed record PartPriceChangedEvent(PartId PartId, Money NewPrice) : IDomainEvent;
