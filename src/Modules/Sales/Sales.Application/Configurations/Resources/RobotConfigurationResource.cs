using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Configurations.Enums;

namespace RoboForge.Modules.Sales.Application.Configurations.Resources;

/// <summary>A robot configuration as the API returns it.</summary>
public sealed record RobotConfigurationResource(Guid Id, RobotConfigurationStatusEnum Status, IReadOnlyList<PartSnapshotResource> Parts)
{
    public static RobotConfigurationResource FromAggregate(RobotConfigurationAggregate configuration) => new(
        configuration.Id.Value,
        configuration.Status,
        [.. configuration.Parts.Select(PartSnapshotResource.FromSnapshot)]);
}
