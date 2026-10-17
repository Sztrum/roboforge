namespace RoboForge.Modules.Sales.Domain.Configurations.Enums;

public enum RobotConfigurationStatusEnum
{
    /// <summary>The customer is still choosing parts.</summary>
    Draft,

    /// <summary>An order was placed from it; it can no longer change.</summary>
    Ordered,
}
