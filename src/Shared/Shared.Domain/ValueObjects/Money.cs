using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>
/// An amount of money in a given currency, e.g. 149.99 PLN. Immutable; two instances with the
/// same amount and currency are equal.
/// </summary>
public sealed record Money
{
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
}
