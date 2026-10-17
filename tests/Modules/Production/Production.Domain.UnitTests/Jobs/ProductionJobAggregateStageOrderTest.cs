using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Modules.Production.Domain.Jobs.Events;
using RoboForge.Modules.Production.Domain.Jobs.Exceptions;
using RoboForge.Modules.Production.Domain.Jobs.ValueObjects;
using static RoboForge.Modules.Production.Domain.UnitTests.Jobs.ProductionJobAggregateExamples;

namespace RoboForge.Modules.Production.Domain.UnitTests.Jobs;

public class ProductionJobAggregateStageOrderTest
{
    [Fact]
    public void StartForOrder_ConfirmedOrder_StartsWithPrinting()
    {
        var orderId = new SalesOrderId(Guid.NewGuid());
        ProductionPart[] parts = [new(Guid.NewGuid(), "Kamera 5 MP", "Sensor")];

        var job = ProductionJobAggregate.StartForOrder(orderId, parts, StartedOnUtc);

        job.OrderId.ShouldBe(orderId);
        job.CurrentStage.ShouldBe(ProductionStageEnum.Printing);
        job.Status.ShouldBe(ProductionJobStatusEnum.InProgress);
        job.Parts.ShouldBe(parts);
    }

    [Fact]
    public void CompleteStage_CurrentStage_MovesToNextAndRaisesProductionStageCompletedEvent()
    {
        var job = StartedJob();

        job.CompleteStage(ProductionStageEnum.Printing, Technician, StartedOnUtc);

        job.CurrentStage.ShouldBe(ProductionStageEnum.Assembly);
        job.CompletedStages.ShouldHaveSingleItem().CompletedBy.ShouldBe(Technician);
        var completedEvent = job.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<ProductionStageCompletedEvent>();
        completedEvent.Stage.ShouldBe(ProductionStageEnum.Printing);
        completedEvent.OrderId.ShouldBe(job.OrderId);
    }

    [Theory]
    [InlineData(ProductionStageEnum.Assembly)]
    [InlineData(ProductionStageEnum.Packing)]
    public void CompleteStage_LaterStage_ThrowsProductionStageCannotBeSkippedException(ProductionStageEnum skippedToStage)
    {
        var job = StartedJob();

        var exception = Should.Throw<ProductionStageCannotBeSkippedException>(
            () => job.CompleteStage(skippedToStage, Technician, StartedOnUtc));

        exception.Rule.ShouldBe("R9");
    }

    [Fact]
    public void CompleteStage_AlreadyCompletedStage_ThrowsProductionStageCannotBeSkippedException()
    {
        var job = JobAt(ProductionStageEnum.Assembly);

        Should.Throw<ProductionStageCannotBeSkippedException>(
            () => job.CompleteStage(ProductionStageEnum.Printing, Technician, StartedOnUtc));
    }

    [Fact]
    public void CompleteStage_ShippedStage_ThrowsShippedStageIsCompletedByShippingException()
    {
        var job = JobAt(ProductionStageEnum.Shipped);

        var exception = Should.Throw<ShippedStageIsCompletedByShippingException>(
            () => job.CompleteStage(ProductionStageEnum.Shipped, Technician, StartedOnUtc));

        exception.Rule.ShouldBe("P1");
    }

    [Fact]
    public void Ship_PackedRobot_ShipsAndRaisesRobotShippedEvent()
    {
        var job = JobAt(ProductionStageEnum.Shipped);

        job.Ship("RF-123456", StartedOnUtc.AddDays(5));

        job.Status.ShouldBe(ProductionJobStatusEnum.Shipped);
        job.TrackingNumber.ShouldBe("RF-123456");
        job.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<RobotShippedEvent>().TrackingNumber.ShouldBe("RF-123456");
    }

    [Fact]
    public void Ship_BeforePacking_ThrowsProductionStageCannotBeSkippedException()
    {
        var job = JobAt(ProductionStageEnum.Packing);

        var exception = Should.Throw<ProductionStageCannotBeSkippedException>(() => job.Ship("RF-1", StartedOnUtc));

        exception.Rule.ShouldBe("R9");
    }

    [Fact]
    public void CompleteStage_ShippedJob_ThrowsProductionJobIsAlreadyShippedException()
    {
        var job = JobAt(ProductionStageEnum.Shipped);
        job.Ship("RF-123456", StartedOnUtc);

        var exception = Should.Throw<ProductionJobIsAlreadyShippedException>(
            () => job.CompleteStage(ProductionStageEnum.Shipped, Technician, StartedOnUtc));

        exception.Rule.ShouldBe("P4");
    }
}
