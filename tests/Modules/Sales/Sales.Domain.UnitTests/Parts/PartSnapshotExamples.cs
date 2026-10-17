using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Parts;

/// <summary>Snapshots of the starter catalog parts, so tests read like the demo.</summary>
internal static class PartSnapshotExamples
{
    public static PartSnapshot RangerHousing() =>
        Snapshot("Obudowa Ranger (PETG)", PartCategoryEnum.Housing, 149m, 3, 320, MountTypeEnum.Standard);

    public static PartSnapshot ScoutHousing() =>
        Snapshot("Obudowa Scout (PLA)", PartCategoryEnum.Housing, 89m, 2, 180, MountTypeEnum.Compact);

    public static PartSnapshot TitanHousing() =>
        Snapshot("Obudowa Titan (ASA)", PartCategoryEnum.Housing, 249m, 5, 650, MountTypeEnum.Heavy);

    public static PartSnapshot TrailDrive() =>
        Snapshot("Napęd Trail 12V", PartCategoryEnum.Drive, 189m, 4, 380, MountTypeEnum.Standard, voltageVolts: 12, maxLoadGrams: 2000);

    public static PartSnapshot MicroDrive() =>
        Snapshot("Napęd Micro 6V", PartCategoryEnum.Drive, 79m, 2, 120, MountTypeEnum.Compact, voltageVolts: 6, maxLoadGrams: 800);

    public static PartSnapshot CrawlerDrive() =>
        Snapshot("Napęd Crawler 12V", PartCategoryEnum.Drive, 399m, 7, 900, MountTypeEnum.Heavy, voltageVolts: 12, maxLoadGrams: 5000);

    public static PartSnapshot ProElectronics() =>
        Snapshot("Sterownik Pro 12V", PartCategoryEnum.Electronics, 229m, 5, 90, voltageVolts: 12);

    public static PartSnapshot NanoElectronics() =>
        Snapshot("Sterownik Nano 6V", PartCategoryEnum.Electronics, 99m, 3, 40, voltageVolts: 6);

    public static PartSnapshot EdgeAiElectronics() =>
        Snapshot("Sterownik Edge AI 12V", PartCategoryEnum.Electronics, 549m, 10, 140, voltageVolts: 12);

    public static PartSnapshot Camera() => Snapshot("Kamera 5 MP", PartCategoryEnum.Sensor, 119m, 4, 25);

    public static PartSnapshot Lidar() => Snapshot("Lidar 360°", PartCategoryEnum.Sensor, 459m, 8, 190);

    /// <summary>Ranger + Trail + Pro + camera: compatible, 788.90 PLN, 8 working days.</summary>
    public static PartSnapshot[] CompatibleRangerRobot() => [RangerHousing(), TrailDrive(), ProElectronics(), Camera()];

    private static PartSnapshot Snapshot(
        string name,
        PartCategoryEnum category,
        decimal price,
        int leadTimeWorkingDays,
        int weightGrams,
        MountTypeEnum? mountType = null,
        int? voltageVolts = null,
        int? maxLoadGrams = null) =>
        new(new CatalogPartId(Guid.NewGuid()), name, category, new Money(price, "PLN"), new LeadTime(leadTimeWorkingDays),
            weightGrams, mountType, voltageVolts, maxLoadGrams, IsDiscontinued: false);
}
