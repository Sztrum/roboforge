namespace RoboForge.Domain.Common;

/// <summary>
/// Base class for business-rule violations (R1–R10 in docs/PROJECT.md). Each rule has its own
/// derived exception named after the exact situation, e.g.
/// <c>OrderCannotBeCancelledInProductionException</c>.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>The violated rule ID, e.g. <c>R8</c>.</summary>
    public string Rule { get; }
}
