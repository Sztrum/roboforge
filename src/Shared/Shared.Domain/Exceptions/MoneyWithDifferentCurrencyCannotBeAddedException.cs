using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.Exceptions;

/// <summary>Thrown when two <see cref="Money"/> amounts in different currencies are added (rule M3).</summary>
public sealed class MoneyWithDifferentCurrencyCannotBeAddedException(string leftCurrency, string rightCurrency)
    : DomainException("M3", $"Nie można dodać kwot w różnych walutach ({leftCurrency} i {rightCurrency}).");
