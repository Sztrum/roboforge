using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.Events;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Aggregates;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.Parts;

/// <summary>
/// A part customers can buy. Its price can change until it is discontinued; discontinuing is final
/// (decision of 2026-10-14).
/// </summary>
public sealed class PartAggregate : AggregateRoot<PartId>
{
    private PartAggregate(PartId id, string name, PartCategoryEnum category, Money price, LeadTime leadTime, TechSpec techSpec)
        : base(id)
    {
        Name = name;
        Category = category;
        Price = price;
        LeadTime = leadTime;
        TechSpec = techSpec;
    }

    public string Name { get; private set; }

    public PartCategoryEnum Category { get; private set; }

    public Money Price { get; private set; }

    public LeadTime LeadTime { get; private set; }

    public TechSpec TechSpec { get; private set; }

    public bool IsDiscontinued { get; private set; }

    public static PartAggregate Create(string name, PartCategoryEnum category, Money price, LeadTime leadTime, TechSpec techSpec)
    {
        var trimmedName = name.Trim();

        if (trimmedName.Length == 0)
        {
            throw new PartNameCannotBeEmptyException();
        }

        GuardTechSpecHasValuesRequiredByCategory(category, techSpec);

        var part = new PartAggregate(PartId.New(), trimmedName, category, price, leadTime, techSpec);
        part.AddDomainEvent(new PartCreatedEvent(part.Id, part.Name, category, price, leadTime, techSpec));

        return part;
    }

    public void ChangePrice(Money newPrice)
    {
        if (IsDiscontinued)
        {
            throw new DiscontinuedPartCannotBeRepricedException(Id);
        }

        if (newPrice == Price)
        {
            return;
        }

        Price = newPrice;
        AddDomainEvent(new PartPriceChangedEvent(Id, newPrice));
    }

    public void Discontinue()
    {
        if (IsDiscontinued)
        {
            throw new PartIsAlreadyDiscontinuedException(Id);
        }

        IsDiscontinued = true;
        AddDomainEvent(new PartDiscontinuedEvent(Id));
    }

    private static void GuardTechSpecHasValuesRequiredByCategory(PartCategoryEnum category, TechSpec techSpec)
    {
        var needsMountType = category is PartCategoryEnum.Housing or PartCategoryEnum.Drive;
        var needsVoltage = category is PartCategoryEnum.Drive or PartCategoryEnum.Electronics;
        var needsMaxLoad = category is PartCategoryEnum.Drive;

        if (needsMountType && techSpec.MountType is null)
        {
            throw new PartTechSpecIsMissingValueForCategoryException(category, nameof(TechSpec.MountType));
        }

        if (needsVoltage && techSpec.VoltageVolts is null)
        {
            throw new PartTechSpecIsMissingValueForCategoryException(category, nameof(TechSpec.VoltageVolts));
        }

        if (needsMaxLoad && techSpec.MaxLoadGrams is null)
        {
            throw new PartTechSpecIsMissingValueForCategoryException(category, nameof(TechSpec.MaxLoadGrams));
        }
    }
}
