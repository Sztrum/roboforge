using System.Reflection;

namespace RoboForge.Architecture.Tests;

internal static class LayerAssemblies
{
    public static readonly Assembly Domain = Assembly.Load("RoboForge.Domain");
    public static readonly Assembly Application = Assembly.Load("RoboForge.Application");
    public static readonly Assembly Infrastructure = Assembly.Load("RoboForge.Infrastructure");
    public static readonly Assembly Web = Assembly.Load("RoboForge.Web");
}
