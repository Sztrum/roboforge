using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Orders.Enums;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Orders;

internal static class OrderAggregateExamples
{
    public static readonly DateTime PlacedOnUtc = new(2026, 10, 17, 9, 0, 0, DateTimeKind.Utc);

    public static RobotConfigurationAggregate CompatibleConfiguration()
    {
        var configuration = RobotConfigurationAggregate.Create();
        foreach (var part in CompatibleRangerRobot())
        {
            configuration.AddPart(part);
        }

        return configuration;
    }

    public static OrderAggregate PlacedOrder() => OrderAggregate.Place(CompatibleConfiguration(), PlacedOnUtc);

    public static OrderAggregate ConfirmedOrder()
    {
        var order = PlacedOrder();
        order.Confirm();
        order.ClearDomainEvents();

        return order;
    }

    public static OrderAggregate OrderInProduction()
    {
        var order = ConfirmedOrder();
        order.MarkStageReached(ProductionStageEnum.Printing, PlacedOnUtc.AddDays(1));

        return order;
    }
}
