using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Catalog.IntegrationEvents;

/// <summary>A part was added to the catalog. Enum values travel as their names.</summary>
public sealed record PartCreatedIntegrationEvent(
    Guid PartId,
    string Name,
    string Category,
    decimal PriceAmount,
    string PriceCurrency,
    int LeadTimeWorkingDays,
    int WeightGrams,
    string? MountType,
    int? VoltageVolts,
    int? MaxLoadGrams) : IntegrationEvent;
