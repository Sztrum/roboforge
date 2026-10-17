using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Parts.ValueObjects;

/// <summary>
/// A copy of a part's price and technical values at the moment it was added to a configuration, so a
/// later catalog change does not change a configuration or order (docs/PROJECT.md, key decision).
/// </summary>
public sealed record PartSnapshot(
    CatalogPartId PartId,
    string Name,
    PartCategoryEnum Category,
    Money Price,
    LeadTime LeadTime,
    int WeightGrams,
    MountTypeEnum? MountType,
    int? VoltageVolts,
    int? MaxLoadGrams,
    bool IsDiscontinued)
{
    /// <summary>The same snapshot, marked as a part the catalog no longer offers (R7).</summary>
    public PartSnapshot AsDiscontinued() => this with { IsDiscontinued = true };
}
