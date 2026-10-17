using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Modules.Sales.Domain.Orders.Exceptions;
using RoboForge.Modules.Sales.Domain.Orders.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;
using static RoboForge.Modules.Sales.Domain.UnitTests.Orders.OrderAggregateExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Orders;

public class OrderAggregateProductionProgressTest
{
    private static readonly DateTime StageCompletedOnUtc = PlacedOnUtc.AddDays(2);

    [Fact]
    public void MarkStageReached_ConfirmedOrder_PutsItInProductionAndRecordsStage()
    {
        var order = ConfirmedOrder();

        order.MarkStageReached(ProductionStageEnum.Printing, StageCompletedOnUtc);

        order.Status.ShouldBe(OrderStatusEnum.InProduction);
        order.CompletedStages.ShouldBe([new OrderStageEntry(ProductionStageEnum.Printing, StageCompletedOnUtc)]);
    }

    [Fact]
    public void MarkStageReached_PlacedOrder_ThrowsOrderIsNotInProductionException()
    {
        var order = PlacedOrder();

        var exception = Should.Throw<OrderIsNotInProductionException>(
            () => order.MarkStageReached(ProductionStageEnum.Printing, StageCompletedOnUtc));

        exception.Rule.ShouldBe("S5");
    }

    [Fact]
    public void RecordFailedQa_OrderInProduction_AddsNoteAndExtendsLeadTimeByThreeDays()
    {
        var order = OrderInProduction();

        order.RecordFailedQa("Luźne mocowanie napędu", StageCompletedOnUtc);

        order.Notes.ShouldHaveSingleItem().Text.ShouldContain("Luźne mocowanie napędu");
        order.LeadTime.ShouldBe(new LeadTime(8 + 3));
    }

    [Fact]
    public void MarkShipped_OrderInProduction_ShipsWithTrackingNumber()
    {
        var order = OrderInProduction();

        order.MarkShipped("RF-123456", StageCompletedOnUtc);

        order.Status.ShouldBe(OrderStatusEnum.Shipped);
        order.TrackingNumber.ShouldBe("RF-123456");
        order.CompletedStages[^1].Stage.ShouldBe(ProductionStageEnum.Shipped);
    }

    [Fact]
    public void MarkShipped_ShippedOrder_ThrowsOrderIsNotInProductionException()
    {
        var order = OrderInProduction();
        order.MarkShipped("RF-123456", StageCompletedOnUtc);

        Should.Throw<OrderIsNotInProductionException>(() => order.MarkShipped("RF-999", StageCompletedOnUtc));
    }
}
