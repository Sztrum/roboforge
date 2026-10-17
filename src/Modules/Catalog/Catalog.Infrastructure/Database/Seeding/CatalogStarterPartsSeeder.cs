using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Infrastructure.Database.Seeding;

/// <summary>
/// Fills an empty catalog with 12 starter parts, three per category, chosen so the demo can show
/// both a compatible robot and every compatibility rule (R2–R4) failing.
/// </summary>
internal static class CatalogStarterPartsSeeder
{
    private const string Currency = "PLN";

    public static async Task SeedWhenEmptyAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        var parts = dbContext.Set<PartAggregate>();

        if (await parts.AnyAsync(cancellationToken))
        {
            return;
        }

        parts.AddRange(CreateStarterParts());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<PartAggregate> CreateStarterParts()
    {
        yield return Housing("Obudowa Scout (PLA)", 89m, 2, 180, MountTypeEnum.Compact);
        yield return Housing("Obudowa Ranger (PETG)", 149m, 3, 320, MountTypeEnum.Standard);
        yield return Housing("Obudowa Titan (ASA)", 249m, 5, 650, MountTypeEnum.Heavy);

        yield return Drive("Napęd Micro 6V", 79m, 2, 120, MountTypeEnum.Compact, voltageVolts: 6, maxLoadGrams: 800);
        yield return Drive("Napęd Trail 12V", 189m, 4, 380, MountTypeEnum.Standard, voltageVolts: 12, maxLoadGrams: 2000);
        yield return Drive("Napęd Crawler 12V", 399m, 7, 900, MountTypeEnum.Heavy, voltageVolts: 12, maxLoadGrams: 5000);

        yield return Electronics("Sterownik Nano 6V", 99m, 3, 40, voltageVolts: 6);
        yield return Electronics("Sterownik Pro 12V", 229m, 5, 90, voltageVolts: 12);
        yield return Electronics("Sterownik Edge AI 12V", 549m, 10, 140, voltageVolts: 12);

        yield return Sensor("Czujnik odległości ultradźwiękowy", 29m, 1, 15);
        yield return Sensor("Kamera 5 MP", 119m, 4, 25);
        yield return Sensor("Lidar 360°", 459m, 8, 190);
    }

    private static PartAggregate Housing(string name, decimal price, int leadTimeDays, int weightGrams, MountTypeEnum mountType) =>
        PartAggregate.Create(name, PartCategoryEnum.Housing, new Money(price, Currency), new LeadTime(leadTimeDays),
            new TechSpec(weightGrams, mountType));

    private static PartAggregate Drive(string name, decimal price, int leadTimeDays, int weightGrams, MountTypeEnum mountType, int voltageVolts, int maxLoadGrams) =>
        PartAggregate.Create(name, PartCategoryEnum.Drive, new Money(price, Currency), new LeadTime(leadTimeDays),
            new TechSpec(weightGrams, mountType, voltageVolts, maxLoadGrams));

    private static PartAggregate Electronics(string name, decimal price, int leadTimeDays, int weightGrams, int voltageVolts) =>
        PartAggregate.Create(name, PartCategoryEnum.Electronics, new Money(price, Currency), new LeadTime(leadTimeDays),
            new TechSpec(weightGrams, voltageVolts: voltageVolts));

    private static PartAggregate Sensor(string name, decimal price, int leadTimeDays, int weightGrams) =>
        PartAggregate.Create(name, PartCategoryEnum.Sensor, new Money(price, Currency), new LeadTime(leadTimeDays),
            new TechSpec(weightGrams));
}
