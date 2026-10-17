namespace RoboForge.Modules.Production.Domain.Jobs;

public readonly record struct ProductionJobId(Guid Value)
{
    public static ProductionJobId New() => new(Guid.CreateVersion7());
}
