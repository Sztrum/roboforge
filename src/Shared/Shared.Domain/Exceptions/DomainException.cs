using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.Exceptions;

/// <summary>
/// Base class for business-rule violations (R1–R10 in docs/PROJECT.md). Each rule has its own
/// derived exception named after the exact situation, e.g.
/// <c>OrderCannotBeCancelledInProductionException</c>. Domain services that collect every problem
/// at once throw one exception carrying several <see cref="Violations"/>.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string rule, string message)
        : base(message)
    {
        Violations = [new RuleViolation(rule, message)];
    }

    protected DomainException(IReadOnlyList<RuleViolation> violations)
        : base(string.Join("; ", violations))
    {
        if (violations.Count == 0)
        {
            throw new ArgumentException("A domain exception needs at least one violated rule.", nameof(violations));
        }

        Violations = violations;
    }

    /// <summary>The (first) violated rule ID, e.g. <c>R8</c>.</summary>
    public string Rule => Violations[0].Rule;

    /// <summary>Every violated rule with its message.</summary>
    public IReadOnlyList<RuleViolation> Violations { get; }
}
