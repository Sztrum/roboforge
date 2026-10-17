using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;

namespace RoboForge.Modules.Catalog.Application.Parts.Resources;

/// <summary>A catalog part as the API returns it.</summary>
public sealed record PartResource(
    Guid Id,
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
    public static PartResource FromAggregate(PartAggregate part) => new(
        part.Id.Value,
        part.Name,
        part.Category,
        part.Price.Amount,
        part.Price.Currency,
        part.LeadTime.WorkingDays,
        part.TechSpec.WeightGrams,
        part.TechSpec.MountType,
        part.TechSpec.VoltageVolts,
        part.TechSpec.MaxLoadGrams,
        part.IsDiscontinued);
}
