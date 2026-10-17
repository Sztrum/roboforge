using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;

/// <summary>What a configuration would cost and take to build, and which rules it still breaks.</summary>
public sealed record RobotQuote(IReadOnlyList<RuleViolation> Violations, RobotPrice Price, LeadTime LeadTime)
{
    public bool IsValid => Violations.Count == 0;
}
