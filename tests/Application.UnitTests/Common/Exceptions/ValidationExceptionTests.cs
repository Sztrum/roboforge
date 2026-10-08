using FluentValidation.Results;
using RoboForge.Application.Common.Exceptions;

namespace RoboForge.Application.UnitTests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithoutFailures_CreatesEmptyErrorDictionary()
    {
        var actual = new ValidationException().Errors;

        actual.Keys.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_WithSingleFailure_CreatesSingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be over 18"),
        };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.ShouldBe(["Age"]);
        actual["Age"].ShouldBe(["must be over 18"]);
    }

    [Fact]
    public void Constructor_WithFailuresForMultipleProperties_GroupsErrorsByProperty()
    {
        var failures = new List<ValidationFailure>
        {
            new("Age", "must be 18 or older"),
            new("Age", "must be 25 or younger"),
            new("Password", "must contain at least 8 characters"),
            new("Password", "must contain a digit"),
            new("Password", "must contain upper case letter"),
            new("Password", "must contain lower case letter"),
        };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.ShouldBe(["Password", "Age"], ignoreOrder: true);

        actual["Age"].ShouldBe(
            ["must be 25 or younger", "must be 18 or older"],
            ignoreOrder: true);

        actual["Password"].ShouldBe(
            [
                "must contain lower case letter",
                "must contain upper case letter",
                "must contain at least 8 characters",
                "must contain a digit",
            ],
            ignoreOrder: true);
    }
}
