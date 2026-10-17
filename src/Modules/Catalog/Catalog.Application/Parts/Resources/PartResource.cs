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
    bool IsDiscontinued);
