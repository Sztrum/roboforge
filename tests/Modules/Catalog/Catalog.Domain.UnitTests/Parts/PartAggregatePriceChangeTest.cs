using RoboForge.Modules.Catalog.Domain.Parts.Events;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts;

public class PartAggregatePriceChangeTest
{
    [Fact]
    public void ChangePrice_WithNewPrice_ChangesPriceAndRaisesPartPriceChangedEvent()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();
        drive.ClearDomainEvents();
        var discountedPrice = new Money(169m, "PLN");

        drive.ChangePrice(discountedPrice);

        drive.Price.ShouldBe(discountedPrice);
        var priceChangedEvent = drive.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<PartPriceChangedEvent>();
        priceChangedEvent.NewPrice.ShouldBe(discountedPrice);
    }

    [Fact]
    public void ChangePrice_WithSamePrice_RaisesNoEvent()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();
        drive.ClearDomainEvents();

        drive.ChangePrice(drive.Price);

        drive.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void ChangePrice_OfDiscontinuedPart_ThrowsDiscontinuedPartCannotBeRepricedException()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();
        drive.Discontinue();

        var exception = Should.Throw<DiscontinuedPartCannotBeRepricedException>(
            () => drive.ChangePrice(new Money(99m, "PLN")));

        exception.Rule.ShouldBe("C4");
    }
}
