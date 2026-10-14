using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class MoneyCreationTest
{
    [Fact]
    public void Constructor_WithPositiveAmount_KeepsAmountAndCurrency()
    {
        var money = new Money(149.99m, "PLN");

        money.Amount.ShouldBe(149.99m);
        money.Currency.ShouldBe("PLN");
    }

    [Fact]
    public void Constructor_WithZeroAmount_IsAllowed()
    {
        var money = new Money(0m, "PLN");

        money.Amount.ShouldBe(0m);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsMoneyAmountCannotBeNegativeException()
    {
        var exception = Should.Throw<MoneyAmountCannotBeNegativeException>(() => new Money(-0.01m, "PLN"));

        exception.Rule.ShouldBe("M1");
    }

    [Fact]
    public void Equality_WithSameAmountAndCurrency_IsTrue()
    {
        var first = new Money(10m, "PLN");
        var second = new Money(10m, "PLN");

        first.ShouldBe(second);
    }
}
