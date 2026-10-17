namespace RoboForge.Modules.Production.Domain.Jobs.Enums;

public enum ProductionJobStatusEnum
{
    InProgress,

    /// <summary>QA failed twice; the workshop must decide before work continues (R10).</summary>
    OnHold,

    Shipped,
}
