using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.Exceptions;

/// <summary>Thrown when an amount in another currency is subtracted from <see cref="Money"/> (rule M3).</summary>
public sealed class MoneyWithDifferentCurrencyCannotBeSubtractedException(string leftCurrency, string rightCurrency)
    : DomainException("M3", $"Nie można odjąć kwot w różnych walutach ({leftCurrency} i {rightCurrency}).");
