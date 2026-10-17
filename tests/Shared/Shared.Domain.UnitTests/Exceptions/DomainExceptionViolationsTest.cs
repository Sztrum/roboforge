using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.Exceptions;

public class DomainExceptionViolationsTest
{
    private sealed class SingleRuleBrokenException() : DomainException("X1", "Pierwsza reguła.");

    private sealed class SeveralRulesBrokenException(IReadOnlyList<RuleViolation> violations) : DomainException(violations);

    [Fact]
    public void Constructor_WithOneRule_ListsItAsTheOnlyViolation()
    {
        var exception = new SingleRuleBrokenException();

        exception.Rule.ShouldBe("X1");
        exception.Violations.ShouldBe([new RuleViolation("X1", "Pierwsza reguła.")]);
    }

    [Fact]
    public void Constructor_WithSeveralViolations_KeepsAllAndJoinsMessages()
    {
        var exception = new SeveralRulesBrokenException([new("R2", "Mocowanie."), new("R4", "Masa.")]);

        exception.Rule.ShouldBe("R2");
        exception.Violations.Count.ShouldBe(2);
        exception.Message.ShouldBe("R2: Mocowanie.; R4: Masa.");
    }

    [Fact]
    public void Constructor_WithoutViolations_Throws()
    {
        Should.Throw<ArgumentException>(() => new SeveralRulesBrokenException([]));
    }
}
