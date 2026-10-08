using System.Reflection;

namespace RoboForge.Architecture.Tests;

/// <summary>
/// The assemblies under test. A new module is added to <see cref="ExistingModules"/> and referenced
/// in the project file.
/// </summary>
internal static class ProjectAssemblies
{
    /// <summary>Modules that exist now; Sales and Production are added in their stages.</summary>
    public static readonly string[] ExistingModules = ["Catalog"];

    /// <summary>Every bounded context, including those not built yet, so isolation rules already name them.</summary>
    public static readonly string[] AllBoundedContexts = ["Catalog", "Sales", "Production"];

    public static readonly string[] ModuleLayers = ["Domain", "Application", "Infrastructure", "UI"];

    public static readonly Assembly[] SharedAssemblies =
    [
        Assembly.Load("RoboForge.Shared.Domain"),
        Assembly.Load("RoboForge.Shared.Application"),
        Assembly.Load("RoboForge.Shared.UI"),
    ];

    public static Assembly ModuleLayer(string module, string layer) =>
        Assembly.Load($"RoboForge.Modules.{module}.{layer}");

    public static string ModuleLayerNamespace(string module, string layer) =>
        $"RoboForge.Modules.{module}.{layer}";
}
