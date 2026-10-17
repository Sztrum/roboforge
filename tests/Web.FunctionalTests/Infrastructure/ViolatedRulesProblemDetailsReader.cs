using System.Text.Json;

namespace RoboForge.Web.FunctionalTests.Infrastructure;

/// <summary>Reads the <c>rules</c> extension of a 422 ProblemDetails response.</summary>
internal static class ViolatedRulesProblemDetailsReader
{
    public static async Task<string[]> ReadViolatedRulesAsync(this HttpResponseMessage response)
    {
        using var problemDetails = JsonDocument.Parse(
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        return [.. problemDetails.RootElement.GetProperty("rules").EnumerateArray().Select(rule => rule.GetString()!)];
    }
}
