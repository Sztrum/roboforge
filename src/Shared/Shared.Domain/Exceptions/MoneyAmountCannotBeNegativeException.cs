using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.Exceptions;

/// <summary>Thrown when <see cref="Money"/> is created with an amount below zero (rule M1).</summary>
public sealed class MoneyAmountCannotBeNegativeException(decimal amount)
    : DomainException("M1", $"Kwota nie może być ujemna (podano {amount}).");
