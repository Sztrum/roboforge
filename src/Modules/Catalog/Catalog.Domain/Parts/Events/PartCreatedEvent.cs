using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.Parts.Events;

public sealed record PartCreatedEvent(
    PartId PartId,
    string Name,
    PartCategoryEnum Category,
    Money Price,
    LeadTime LeadTime,
    TechSpec TechSpec) : IDomainEvent;
