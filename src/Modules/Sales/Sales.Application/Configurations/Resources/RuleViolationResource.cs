using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Application.Configurations.Resources;

/// <summary>A business rule the configuration still breaks.</summary>
public sealed record RuleViolationResource(string Rule, string Message)
{
    public static RuleViolationResource FromViolation(RuleViolation violation) => new(violation.Rule, violation.Message);
}
