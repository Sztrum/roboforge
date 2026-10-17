using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts.ValueObjects;

public class TechSpecCreationTest
{
    [Fact]
    public void Constructor_WithAllValues_KeepsThem()
    {
        var driveSpec = new TechSpec(weightGrams: 380, mountType: MountTypeEnum.Standard, voltageVolts: 12, maxLoadGrams: 2000);

        driveSpec.WeightGrams.ShouldBe(380);
        driveSpec.MountType.ShouldBe(MountTypeEnum.Standard);
        driveSpec.VoltageVolts.ShouldBe(12);
        driveSpec.MaxLoadGrams.ShouldBe(2000);
    }

    [Fact]
    public void Constructor_WithWeightOnly_LeavesOtherValuesEmpty()
    {
        var sensorSpec = new TechSpec(weightGrams: 15);

        sensorSpec.MountType.ShouldBeNull();
        sensorSpec.VoltageVolts.ShouldBeNull();
        sensorSpec.MaxLoadGrams.ShouldBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveWeight_ThrowsTechSpecValueMustBePositiveException(int invalidWeightGrams)
    {
        var exception = Should.Throw<TechSpecValueMustBePositiveException>(() => new TechSpec(invalidWeightGrams));

        exception.Rule.ShouldBe("C2");
    }

    [Fact]
    public void Constructor_WithNonPositiveVoltage_ThrowsTechSpecValueMustBePositiveException()
    {
        var exception = Should.Throw<TechSpecValueMustBePositiveException>(
            () => new TechSpec(weightGrams: 40, voltageVolts: 0));

        exception.Rule.ShouldBe("C2");
    }

    [Fact]
    public void Constructor_WithNonPositiveMaxLoad_ThrowsTechSpecValueMustBePositiveException()
    {
        var exception = Should.Throw<TechSpecValueMustBePositiveException>(
            () => new TechSpec(weightGrams: 120, maxLoadGrams: -5));

        exception.Rule.ShouldBe("C2");
    }

    [Fact]
    public void Equality_WithSameValues_IsTrue()
    {
        var electronicsSpec = new TechSpec(weightGrams: 40, voltageVolts: 6);
        var sameElectronicsSpec = new TechSpec(weightGrams: 40, voltageVolts: 6);

        electronicsSpec.ShouldBe(sameElectronicsSpec);
    }
}
