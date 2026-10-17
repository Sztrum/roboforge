using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts;

/// <summary>Ready-made valid parts, so each test only states what matters to it.</summary>
internal static class PartAggregateExamples
{
    public static readonly TechSpec TrailDriveSpec =
        new(weightGrams: 380, mountType: MountTypeEnum.Standard, voltageVolts: 12, maxLoadGrams: 2000);

    public static PartAggregate CreateTrailDrive() =>
        PartAggregate.Create("Napęd Trail 12V", PartCategoryEnum.Drive, new Money(189m, "PLN"), new LeadTime(4), TrailDriveSpec);
}
