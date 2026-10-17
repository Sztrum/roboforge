using RoboForge.Modules.Sales.Domain.Policies;
using RoboForge.Modules.Sales.Domain.UnitTests.Parts;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Policies;

public class RobotPartsCompatibilityPolicyTest
{
    [Fact]
    public void FindViolations_WithCompatibleParts_ReturnsNone()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations(CompatibleRangerRobot());

        violations.ShouldBeEmpty();
    }

    [Fact]
    public void FindViolations_WithDriveOfOtherMountType_ReturnsR2()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([TitanHousing(), TrailDrive(), ProElectronics()]);

        violations.Select(violation => violation.Rule).ShouldBe(["R2"]);
    }

    [Fact]
    public void FindViolations_WithElectronicsOfOtherVoltage_ReturnsR3()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([RangerHousing(), TrailDrive(), NanoElectronics()]);

        violations.Select(violation => violation.Rule).ShouldBe(["R3"]);
    }

    [Fact]
    public void FindViolations_WithPartsHeavierThanDriveCanCarry_ReturnsR4()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([ScoutHousing(), MicroDrive(), NanoElectronics(), Lidar(), Lidar(), Lidar()]);

        violations.Select(violation => violation.Rule).ShouldBe(["R4"]);
    }

    [Fact]
    public void FindViolations_WithSeveralProblems_ReturnsEveryViolationAtOnce()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([TitanHousing(), MicroDrive(), ProElectronics()]);

        violations.Select(violation => violation.Rule).ShouldBe(["R2", "R3", "R4"]);
    }

    [Fact]
    public void FindViolations_WithoutDrive_ChecksNothingThatNeedsIt()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([TitanHousing(), NanoElectronics()]);

        violations.ShouldBeEmpty();
    }

    [Fact]
    public void FindViolations_WithTwoDrives_LeavesTheProblemToR1()
    {
        var violations = RobotPartsCompatibilityPolicy.FindViolations([RangerHousing(), TrailDrive(), MicroDrive(), ProElectronics()]);

        violations.ShouldBeEmpty();
    }
}
