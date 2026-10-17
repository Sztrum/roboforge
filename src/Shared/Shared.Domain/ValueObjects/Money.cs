using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>
/// An amount of money in a given currency, e.g. 149.99 PLN. Immutable; two instances with the
/// same amount and currency are equal.
/// </summary>
public sealed record Money
{
    /// <summary>The MVP sells in Polish zloty only, but every amount still carries its currency.</summary>
    public const string DefaultCurrency = "PLN";

    private const int DecimalPlacesOfWholeGrosze = 2;

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new MoneyAmountCannotBeNegativeException(amount);
        }

        Amount = decimal.Round(amount, DecimalPlacesOfWholeGrosze, MidpointRounding.AwayFromZero);
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new MoneyWithDifferentCurrencyCannotBeAddedException(left.Currency, right.Currency);
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies by a quantity (3 sensors) or a factor (1.15 for assembly). The result goes through
    /// the constructor, so it is rounded to whole grosze (M2) and cannot be negative (M1).
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>Zero in the given currency; the starting point for summing prices.</summary>
    public static Money Zero(string currency)
    {
        return new Money(0m, currency);
    }
}
