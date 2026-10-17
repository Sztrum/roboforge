using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;

namespace RoboForge.Modules.Sales.Application.Configurations.Resources;

/// <summary>A part in a configuration or order, as copied when it was added.</summary>
public sealed record PartSnapshotResource(
    Guid PartId,
    string Name,
    PartCategoryEnum Category,
    decimal PriceAmount,
    string PriceCurrency,
    int LeadTimeWorkingDays,
    int WeightGrams,
    MountTypeEnum? MountType,
    int? VoltageVolts,
    int? MaxLoadGrams,
    bool IsDiscontinued)
{
    public static PartSnapshotResource FromSnapshot(PartSnapshot part) => new(
        part.PartId.Value,
        part.Name,
        part.Category,
        part.Price.Amount,
        part.Price.Currency,
        part.LeadTime.WorkingDays,
        part.WeightGrams,
        part.MountType,
        part.VoltageVolts,
        part.MaxLoadGrams,
        part.IsDiscontinued);
}
