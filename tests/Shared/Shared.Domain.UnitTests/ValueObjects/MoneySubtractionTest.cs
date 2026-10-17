using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class MoneySubtractionTest
{
    [Fact]
    public void Subtraction_WithSameCurrency_ReturnsDifference()
    {
        var priceBeforeDiscount = new Money(1904.40m, "PLN");
        var discount = new Money(95.22m, "PLN");

        var priceAfterDiscount = priceBeforeDiscount - discount;

        priceAfterDiscount.ShouldBe(new Money(1809.18m, "PLN"));
    }

    [Fact]
    public void Subtraction_OfLargerAmount_ThrowsMoneyAmountCannotBeNegativeException()
    {
        var smallerPrice = new Money(10m, "PLN");
        var largerPrice = new Money(20m, "PLN");

        var exception = Should.Throw<MoneyAmountCannotBeNegativeException>(() => smallerPrice - largerPrice);

        exception.Rule.ShouldBe("M1");
    }

    [Fact]
    public void Subtraction_WithDifferentCurrencies_ThrowsMoneyWithDifferentCurrencyCannotBeSubtractedException()
    {
        var priceInZlotys = new Money(100m, "PLN");
        var priceInEuros = new Money(20m, "EUR");

        var exception = Should.Throw<MoneyWithDifferentCurrencyCannotBeSubtractedException>(() => priceInZlotys - priceInEuros);

        exception.Rule.ShouldBe("M3");
    }
}
