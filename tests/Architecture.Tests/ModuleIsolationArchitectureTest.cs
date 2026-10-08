using NetArchTest.Rules;
using static RoboForge.Architecture.Tests.ArchitectureTestResultMessage;
using static RoboForge.Architecture.Tests.ProjectAssemblies;

namespace RoboForge.Architecture.Tests;

/// <summary>
/// Modules (bounded contexts) never use each other's code; they integrate only through events
/// (docs/PROJECT.md, "Domain events and integration"). Shared code knows no module.
/// </summary>
public class ModuleIsolationArchitectureTest
{
    public static TheoryData<string, string, string> ModuleLayersWithOtherContexts() =>
        new(ExistingModules.SelectMany(module => ModuleLayers.SelectMany(layer =>
            AllBoundedContexts.Where(other => other != module).Select(other => (module, layer, other)))));

    [Theory]
    [MemberData(nameof(ModuleLayersWithOtherContexts))]
    public void ModuleLayer_ShouldNotDependOn_OtherModule(string module, string layer, string otherModule)
    {
        var result = Types.InAssembly(ModuleLayer(module, layer))
            .ShouldNot().HaveDependencyOn($"RoboForge.Modules.{otherModule}")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Fact]
    public void SharedCode_ShouldNotDependOn_AnyModule()
    {
        var result = Types.InAssemblies(SharedAssemblies)
            .ShouldNot().HaveDependencyOn("RoboForge.Modules")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Npgsql")]
    [InlineData("Microsoft.AspNetCore")]
    [InlineData("Mediator")]
    [InlineData("FluentValidation")]
    public void SharedDomain_ShouldNotDependOn_Frameworks(string framework)
    {
        var result = Types.InAssembly(SharedAssemblies[0])
            .ShouldNot().HaveDependencyOn(framework)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }
}
