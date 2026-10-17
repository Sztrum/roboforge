using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Configurations.Enums;
using RoboForge.Modules.Sales.Domain.Configurations.Exceptions;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Shared.Domain.ValueObjects;
using static RoboForge.Modules.Sales.Domain.UnitTests.Orders.OrderAggregateExamples;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Orders;

public class OrderAggregatePlacementTest
{
    [Fact]
    public void Place_ValidConfiguration_CopiesPartsPriceAndLeadTime()
    {
        var configuration = CompatibleConfiguration();

        var order = OrderAggregate.Place(configuration, PlacedOnUtc);

        order.Status.ShouldBe(OrderStatusEnum.Placed);
        order.ConfigurationId.ShouldBe(configuration.Id);
        order.Parts.ShouldBe(configuration.Parts);
        order.Price.TotalPrice.ShouldBe(new Money(788.90m, "PLN"));
        order.LeadTime.ShouldBe(new LeadTime(8));
        order.PlacedOnUtc.ShouldBe(PlacedOnUtc);
    }

    [Fact]
    public void Place_ValidConfiguration_LocksTheConfiguration()
    {
        var configuration = CompatibleConfiguration();

        OrderAggregate.Place(configuration, PlacedOnUtc);

        configuration.Status.ShouldBe(RobotConfigurationStatusEnum.Ordered);
    }

    [Fact]
    public void Place_InvalidConfiguration_ThrowsRobotConfigurationIsNotValidException()
    {
        var configuration = RobotConfigurationAggregate.Create();
        configuration.AddPart(TitanHousing());

        var exception = Should.Throw<RobotConfigurationIsNotValidException>(() => OrderAggregate.Place(configuration, PlacedOnUtc));

        exception.Violations.ShouldAllBe(violation => violation.Rule == "R1");
    }

    [Fact]
    public void Place_SameConfigurationTwice_ThrowsOrderedRobotConfigurationCannotBeChangedException()
    {
        var configuration = CompatibleConfiguration();
        OrderAggregate.Place(configuration, PlacedOnUtc);

        var exception = Should.Throw<OrderedRobotConfigurationCannotBeChangedException>(() => OrderAggregate.Place(configuration, PlacedOnUtc));

        exception.Rule.ShouldBe("S1");
    }
}
