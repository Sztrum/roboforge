namespace RoboForge.Domain.Common;

/// <summary>
/// Thrown when a business method would break a business rule (R1–R10 in docs/PROJECT.md).
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>The violated rule ID, e.g. <c>R8</c>.</summary>
    public string Rule { get; }
}
