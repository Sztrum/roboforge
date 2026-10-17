namespace RoboForge.Frontend.Api;

/// <summary>The API refused a request; carries its ProblemDetails message and the violated rule IDs.</summary>
public sealed class ApiRequestRejectedException(string message, IReadOnlyList<string> rules) : Exception(message)
{
    public IReadOnlyList<string> Rules { get; } = rules;
}
