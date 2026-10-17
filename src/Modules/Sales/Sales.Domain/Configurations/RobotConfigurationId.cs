namespace RoboForge.Modules.Sales.Domain.Configurations;

public readonly record struct RobotConfigurationId(Guid Value)
{
    public static RobotConfigurationId New() => new(Guid.CreateVersion7());
}
