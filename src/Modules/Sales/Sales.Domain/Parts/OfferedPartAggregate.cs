using System.Diagnostics.CodeAnalysis;
using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Aggregates;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Parts;

/// <summary>
/// Sales' copy of a catalog part, kept up to date from Catalog's integration events. It is what Sales
/// takes a <see cref="PartSnapshot"/> of when a customer adds the part to a configuration.
/// </summary>
public sealed class OfferedPartAggregate : AggregateRoot<CatalogPartId>
{
    private OfferedPartAggregate(CatalogPartId id, PartSnapshot currentOffer)
        : base(id)
    {
        CurrentOffer = currentOffer;
    }

    [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed",
        Justification = "EF Core calls it through reflection when loading an offered part.")]
    private OfferedPartAggregate(CatalogPartId id)
        : base(id)
    {
        CurrentOffer = null!;
    }

    /// <summary>The part as it is offered right now.</summary>
    public PartSnapshot CurrentOffer { get; private set; }

    public static OfferedPartAggregate CopyFromCatalog(
        CatalogPartId partId,
        string name,
        PartCategoryEnum category,
        Money price,
        LeadTime leadTime,
        int weightGrams,
        MountTypeEnum? mountType,
        int? voltageVolts,
        int? maxLoadGrams) =>
        new(partId, new PartSnapshot(partId, name, category, price, leadTime, weightGrams, mountType, voltageVolts, maxLoadGrams, IsDiscontinued: false));

    public void ChangePrice(Money newPrice) => CurrentOffer = CurrentOffer with { Price = newPrice };

    public void Discontinue() => CurrentOffer = CurrentOffer.AsDiscontinued();

    /// <summary>A copy of the current offer for a configuration; later changes here do not affect it.</summary>
    public PartSnapshot TakeSnapshot() => CurrentOffer;
}
