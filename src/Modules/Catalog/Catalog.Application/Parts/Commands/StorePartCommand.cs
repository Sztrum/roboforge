using Mediator;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands;

/// <summary>Adds a new part to the catalog; the price is in PLN.</summary>
public sealed record StorePartCommand(
    string Name,
    PartCategoryEnum Category,
    decimal PriceAmount,
    int LeadTimeWorkingDays,
    int WeightGrams,
    MountTypeEnum? MountType,
    int? VoltageVolts,
    int? MaxLoadGrams) : ICommand<PartResource>;
