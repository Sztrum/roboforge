using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class MoneyRoundingTest
{
    // Attributes cannot hold decimal constants, and double would distort amounts (10.005 → 10.005000000000001),
    // so the cases are listed as decimals here.
    public static TheoryData<decimal, decimal> AmountsWithTheirRoundedValue() => new()
    {
        { 10.004m, 10.00m },
        { 10.005m, 10.01m },
        { 10.015m, 10.02m },
        { 149.999m, 150.00m },
        { 0.004m, 0.00m },
    };

    [Theory]
    [MemberData(nameof(AmountsWithTheirRoundedValue))]
    public void Constructor_WithMoreThanTwoDecimalPlaces_RoundsToWholeGrosze(
        decimal amountWithFractionOfGrosz,
        decimal expectedAmountInWholeGrosze)
    {
        var money = new Money(amountWithFractionOfGrosz, "PLN");

        money.Amount.ShouldBe(expectedAmountInWholeGrosze);
    }

    [Fact]
    public void Constructor_WithTwoDecimalPlaces_KeepsAmountUnchanged()
    {
        var moneyInWholeGrosze = new Money(149.99m, "PLN");

        moneyInWholeGrosze.Amount.ShouldBe(149.99m);
    }

    [Fact]
    public void Equality_WithAmountsThatRoundToTheSameGrosze_IsTrue()
    {
        var moneyWithFractionOfGrosz = new Money(10.004m, "PLN");
        var moneyInWholeZlotys = new Money(10m, "PLN");

        moneyWithFractionOfGrosz.ShouldBe(moneyInWholeZlotys);
    }
}
