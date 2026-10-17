using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class MoneyMultiplicationTest
{
    [Fact]
    public void Multiplication_ByQuantity_ReturnsPriceOfAllPieces()
    {
        var sensorUnitPrice = new Money(49.99m, "PLN");
        const int sensorQuantity = 3;

        var priceOfAllSensors = sensorUnitPrice * sensorQuantity;

        priceOfAllSensors.ShouldBe(new Money(149.97m, "PLN"));
    }

    [Fact]
    public void Multiplication_ByFactor_RoundsResultToWholeGrosze()
    {
        var partsPrice = new Money(99.99m, "PLN");
        const decimal assemblyMarkupFactor = 1.15m;

        var priceWithAssembly = partsPrice * assemblyMarkupFactor;

        priceWithAssembly.ShouldBe(new Money(114.99m, "PLN"));
    }

    [Fact]
    public void Multiplication_ByZero_ReturnsZeroInSameCurrency()
    {
        var drivePrice = new Money(120m, "PLN");

        var priceOfNoDrives = drivePrice * 0;

        priceOfNoDrives.ShouldBe(new Money(0m, "PLN"));
    }

    [Fact]
    public void Multiplication_ByFactor_LeavesOriginalMoneyUnchanged()
    {
        var housingPrice = new Money(100m, "PLN");

        _ = housingPrice * 2;

        housingPrice.Amount.ShouldBe(100m);
    }

    [Fact]
    public void Multiplication_ByNegativeFactor_ThrowsMoneyAmountCannotBeNegativeException()
    {
        var housingPrice = new Money(100m, "PLN");

        var exception = Should.Throw<MoneyAmountCannotBeNegativeException>(() => housingPrice * -1);

        exception.Rule.ShouldBe("M1");
    }
}
