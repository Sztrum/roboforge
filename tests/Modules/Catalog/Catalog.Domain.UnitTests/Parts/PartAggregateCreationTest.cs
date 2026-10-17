using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.Events;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts;

public class PartAggregateCreationTest
{
    private static readonly Money SensorPrice = new(29m, "PLN");
    private static readonly LeadTime OneWorkingDay = new(1);
    private static readonly TechSpec SensorSpec = new(weightGrams: 15);

    [Fact]
    public void Create_WithValidValues_KeepsThemAndIsNotDiscontinued()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();

        drive.Name.ShouldBe("Napęd Trail 12V");
        drive.Category.ShouldBe(PartCategoryEnum.Drive);
        drive.Price.ShouldBe(new Money(189m, "PLN"));
        drive.LeadTime.ShouldBe(new LeadTime(4));
        drive.TechSpec.ShouldBe(PartAggregateExamples.TrailDriveSpec);
        drive.IsDiscontinued.ShouldBeFalse();
    }

    [Fact]
    public void Create_WithValidValues_RaisesPartCreatedEvent()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();

        var createdEvent = drive.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<PartCreatedEvent>();
        createdEvent.PartId.ShouldBe(drive.Id);
        createdEvent.Price.ShouldBe(drive.Price);
    }

    [Fact]
    public void Create_WithSurroundingSpaces_TrimsName()
    {
        var sensor = PartAggregate.Create("  Kamera 5 MP  ", PartCategoryEnum.Sensor, SensorPrice, OneWorkingDay, SensorSpec);

        sensor.Name.ShouldBe("Kamera 5 MP");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ThrowsPartNameCannotBeEmptyException(string emptyName)
    {
        var exception = Should.Throw<PartNameCannotBeEmptyException>(
            () => PartAggregate.Create(emptyName, PartCategoryEnum.Sensor, SensorPrice, OneWorkingDay, SensorSpec));

        exception.Rule.ShouldBe("C1");
    }

    public static TheoryData<PartCategoryEnum, TechSpec> CategoriesWithIncompleteTechSpec() => new()
    {
        { PartCategoryEnum.Housing, new TechSpec(weightGrams: 320) },
        { PartCategoryEnum.Drive, new TechSpec(weightGrams: 380, voltageVolts: 12, maxLoadGrams: 2000) },
        { PartCategoryEnum.Drive, new TechSpec(weightGrams: 380, mountType: MountTypeEnum.Standard, maxLoadGrams: 2000) },
        { PartCategoryEnum.Drive, new TechSpec(weightGrams: 380, mountType: MountTypeEnum.Standard, voltageVolts: 12) },
        { PartCategoryEnum.Electronics, new TechSpec(weightGrams: 40) },
    };

    [Theory]
    [MemberData(nameof(CategoriesWithIncompleteTechSpec))]
    public void Create_WithTechSpecMissingValueForCategory_ThrowsPartTechSpecIsMissingValueForCategoryException(
        PartCategoryEnum category,
        TechSpec incompleteTechSpec)
    {
        var exception = Should.Throw<PartTechSpecIsMissingValueForCategoryException>(
            () => PartAggregate.Create("Część testowa", category, SensorPrice, OneWorkingDay, incompleteTechSpec));

        exception.Rule.ShouldBe("C3");
    }

    [Fact]
    public void Create_SensorWithWeightOnly_IsAllowed()
    {
        var sensor = PartAggregate.Create("Czujnik odległości", PartCategoryEnum.Sensor, SensorPrice, OneWorkingDay, SensorSpec);

        sensor.TechSpec.ShouldBe(SensorSpec);
    }
}
