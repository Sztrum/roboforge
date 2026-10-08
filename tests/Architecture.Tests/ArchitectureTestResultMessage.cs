using TestResult = NetArchTest.Rules.TestResult;

namespace RoboForge.Architecture.Tests;

internal static class ArchitectureTestResultMessage
{
    public static string ListViolatingTypes(TestResult result) =>
        "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
