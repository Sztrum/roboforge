using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class MoneyAdditionTest
{
    [Fact]
    public void Addition_WithSameCurrency_ReturnsSumOfAmounts()
    {
        var housingPrice = new Money(100.50m, "PLN");
        var drivePrice = new Money(49.49m, "PLN");

        var totalPrice = housingPrice + drivePrice;

        totalPrice.ShouldBe(new Money(149.99m, "PLN"));
    }

    [Fact]
    public void Addition_WithZero_ReturnsSameAmount()
    {
        var sensorPrice = new Money(25m, "PLN");
        var noPrice = new Money(0m, "PLN");

        var totalPrice = sensorPrice + noPrice;

        totalPrice.ShouldBe(sensorPrice);
    }

    [Fact]
    public void Addition_WithSameCurrency_LeavesBothOperandsUnchanged()
    {
        var housingPrice = new Money(100m, "PLN");
        var drivePrice = new Money(50m, "PLN");

        _ = housingPrice + drivePrice;

        housingPrice.Amount.ShouldBe(100m);
        drivePrice.Amount.ShouldBe(50m);
    }

    [Fact]
    public void Addition_WithDifferentCurrencies_ThrowsMoneyWithDifferentCurrencyCannotBeAddedException()
    {
        var priceInZlotys = new Money(100m, "PLN");
        var priceInEuros = new Money(20m, "EUR");

        var exception = Should.Throw<MoneyWithDifferentCurrencyCannotBeAddedException>(
            () => priceInZlotys + priceInEuros);

        exception.Rule.ShouldBe("M3");
    }
}
