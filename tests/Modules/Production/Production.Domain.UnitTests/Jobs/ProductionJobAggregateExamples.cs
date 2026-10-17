using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Modules.Production.Domain.Jobs.ValueObjects;

namespace RoboForge.Modules.Production.Domain.UnitTests.Jobs;

internal static class ProductionJobAggregateExamples
{
    public const string Technician = "Ania (warsztat)";

    public static readonly DateTime StartedOnUtc = new(2026, 10, 17, 8, 0, 0, DateTimeKind.Utc);

    public static ProductionJobAggregate StartedJob()
    {
        var job = ProductionJobAggregate.StartForOrder(
            new SalesOrderId(Guid.NewGuid()),
            [new ProductionPart(Guid.NewGuid(), "Obudowa Ranger (PETG)", "Housing")],
            StartedOnUtc);
        job.ClearDomainEvents();

        return job;
    }

    /// <summary>A job whose stages up to (not including) <paramref name="stage"/> are completed.</summary>
    public static ProductionJobAggregate JobAt(ProductionStageEnum stage)
    {
        var job = StartedJob();

        for (var completedStage = ProductionStageEnum.Printing; completedStage < stage; completedStage++)
        {
            job.CompleteStage(completedStage, Technician, StartedOnUtc.AddHours((int)completedStage + 1));
        }

        job.ClearDomainEvents();

        return job;
    }
}
