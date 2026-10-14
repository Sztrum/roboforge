using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>
/// An amount of money in a given currency, e.g. 149.99 PLN. Immutable; two instances with the
/// same amount and currency are equal.
/// </summary>
public sealed record Money
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new MoneyAmountCannotBeNegativeException(amount);
        }

        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }
}
