using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Parts;

public class OfferedPartAggregateSnapshotTest
{
    private static OfferedPartAggregate OfferedCamera() =>
        OfferedPartAggregate.CopyFromCatalog(
            new CatalogPartId(Guid.NewGuid()), "Kamera 5 MP", PartCategoryEnum.Sensor, new Money(119m, "PLN"), new LeadTime(4), 25, null, null, null);

    [Fact]
    public void TakeSnapshot_AfterPriceChange_HasNewPrice()
    {
        var offeredCamera = OfferedCamera();

        offeredCamera.ChangePrice(new Money(99m, "PLN"));

        offeredCamera.TakeSnapshot().Price.ShouldBe(new Money(99m, "PLN"));
    }

    [Fact]
    public void TakeSnapshot_TakenBeforePriceChange_KeepsOldPrice()
    {
        var offeredCamera = OfferedCamera();
        var snapshotInConfiguration = offeredCamera.TakeSnapshot();

        offeredCamera.ChangePrice(new Money(99m, "PLN"));

        snapshotInConfiguration.Price.ShouldBe(new Money(119m, "PLN"));
    }

    [Fact]
    public void TakeSnapshot_AfterDiscontinue_IsMarkedDiscontinued()
    {
        var offeredCamera = OfferedCamera();

        offeredCamera.Discontinue();

        offeredCamera.TakeSnapshot().IsDiscontinued.ShouldBeTrue();
    }
}
