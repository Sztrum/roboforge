using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoboForge.Web.FunctionalTests.Infrastructure;

/// <summary>Reads API responses the way the API writes them: web defaults with enums as names.</summary>
internal static class ApiJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };
}
