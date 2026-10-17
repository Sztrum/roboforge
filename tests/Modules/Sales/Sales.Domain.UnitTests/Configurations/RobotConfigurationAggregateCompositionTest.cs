using RoboForge.Modules.Sales.Domain.Configurations;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Configurations;

public class RobotConfigurationAggregateCompositionTest
{
    [Fact]
    public void Validate_CompatibleRobot_ReturnsNoViolations()
    {
        var configuration = RobotConfigurationAggregate.Create();
        foreach (var part in CompatibleRangerRobot())
        {
            configuration.AddPart(part);
        }

        configuration.Validate().ShouldBeEmpty();
    }

    [Fact]
    public void Validate_EmptyConfiguration_ReportsMissingHousingDriveAndElectronicsAsR1()
    {
        var emptyConfiguration = RobotConfigurationAggregate.Create();

        var violations = emptyConfiguration.Validate();

        violations.Select(violation => violation.Rule).ShouldBe(["R1", "R1", "R1"]);
    }

    [Fact]
    public void Validate_WithFourSensors_ReportsR1()
    {
        var configuration = RobotConfigurationAggregate.Create();
        foreach (var part in CompatibleRangerRobot())
        {
            configuration.AddPart(part);
        }

        configuration.AddPart(Camera());
        configuration.AddPart(Camera());
        configuration.AddPart(Camera());

        configuration.Validate().ShouldHaveSingleItem().Rule.ShouldBe("R1");
    }

    [Fact]
    public void Validate_WithTwoHousings_ReportsR1()
    {
        var configuration = RobotConfigurationAggregate.Create();
        foreach (var part in CompatibleRangerRobot())
        {
            configuration.AddPart(part);
        }

        configuration.AddPart(RangerHousing());

        configuration.Validate().ShouldHaveSingleItem().Rule.ShouldBe("R1");
    }

    [Fact]
    public void Validate_WithIncompatibleParts_ReportsCompatibilityRules()
    {
        var configuration = RobotConfigurationAggregate.Create();
        configuration.AddPart(TitanHousing());
        configuration.AddPart(MicroDrive());
        configuration.AddPart(NanoElectronics());

        configuration.Validate().Select(violation => violation.Rule).ShouldBe(["R2", "R4"]);
    }
}
