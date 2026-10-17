namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>One broken business rule, e.g. <c>R2</c> with a message for the customer.</summary>
public sealed record RuleViolation(string Rule, string Message)
{
    public override string ToString() => $"{Rule}: {Message}";
}
