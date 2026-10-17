using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Modules.Sales.Domain.Orders.Events;
using RoboForge.Modules.Sales.Domain.Orders.Exceptions;
using static RoboForge.Modules.Sales.Domain.UnitTests.Orders.OrderAggregateExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Orders;

public class OrderAggregateConfirmationAndCancellationTest
{
    [Fact]
    public void Confirm_PlacedOrder_ConfirmsAndRaisesOrderConfirmedEventWithParts()
    {
        var order = PlacedOrder();

        order.Confirm();

        order.Status.ShouldBe(OrderStatusEnum.Confirmed);
        var confirmedEvent = order.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<OrderConfirmedEvent>();
        confirmedEvent.OrderId.ShouldBe(order.Id);
        confirmedEvent.Parts.ShouldBe(order.Parts);
    }

    [Fact]
    public void Confirm_ConfirmedOrder_ThrowsOrderCanBeConfirmedOnlyWhenPlacedException()
    {
        var order = ConfirmedOrder();

        var exception = Should.Throw<OrderCanBeConfirmedOnlyWhenPlacedException>(order.Confirm);

        exception.Rule.ShouldBe("S3");
    }

    [Fact]
    public void Cancel_PlacedOrder_CancelsAndRaisesOrderCancelledEvent()
    {
        var order = PlacedOrder();

        order.Cancel("Zmiana planów");

        order.Status.ShouldBe(OrderStatusEnum.Cancelled);
        order.CancellationReason.ShouldBe("Zmiana planów");
        order.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<OrderCancelledEvent>().Reason.ShouldBe("Zmiana planów");
    }

    [Fact]
    public void Cancel_ConfirmedOrder_ThrowsOrderCannotBeCancelledInProductionException()
    {
        var order = ConfirmedOrder();

        var exception = Should.Throw<OrderCannotBeCancelledInProductionException>(() => order.Cancel("Za późno"));

        exception.Rule.ShouldBe("R8");
    }

    [Fact]
    public void Cancel_OrderInProduction_ThrowsOrderCannotBeCancelledInProductionException()
    {
        var order = OrderInProduction();

        var exception = Should.Throw<OrderCannotBeCancelledInProductionException>(() => order.Cancel("Za późno"));

        exception.Rule.ShouldBe("R8");
    }

    [Fact]
    public void Cancel_CancelledOrder_ThrowsOrderIsAlreadyCancelledException()
    {
        var order = PlacedOrder();
        order.Cancel("Pierwszy raz");

        var exception = Should.Throw<OrderIsAlreadyCancelledException>(() => order.Cancel("Drugi raz"));

        exception.Rule.ShouldBe("S4");
    }

    [Fact]
    public void Confirm_CancelledOrder_ThrowsOrderCanBeConfirmedOnlyWhenPlacedException()
    {
        var order = PlacedOrder();
        order.Cancel("Rezygnacja");

        Should.Throw<OrderCanBeConfirmedOnlyWhenPlacedException>(order.Confirm);
    }
}
