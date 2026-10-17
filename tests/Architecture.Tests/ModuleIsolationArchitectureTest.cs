using NetArchTest.Rules;
using static RoboForge.Architecture.Tests.ArchitectureTestResultMessage;
using static RoboForge.Architecture.Tests.ProjectAssemblies;

namespace RoboForge.Architecture.Tests;

/// <summary>
/// Modules (bounded contexts) never use each other's code; they integrate only through the integration
/// events another module publishes (docs/PROJECT.md, "Domain events and integration"). Shared code knows
/// no module.
/// </summary>
public class ModuleIsolationArchitectureTest
{
    public static TheoryData<string, string, string> ModuleLayersWithOtherContexts() =>
        new(ExistingModules.SelectMany(module => ModuleLayersWithIntegrationEvents.SelectMany(layer =>
            AllBoundedContexts.Where(other => other != module).Select(other => (module, layer, other)))));

    public static TheoryData<string> AllModules() => new(ExistingModules);

    [Theory]
    [MemberData(nameof(ModuleLayersWithOtherContexts))]
    public void ModuleLayer_ShouldNotDependOn_OtherModulesPrivateLayers(string module, string layer, string otherModule)
    {
        var result = Types.InAssembly(ModuleLayer(module, layer))
            .ShouldNot().HaveDependencyOnAny([.. ModuleLayers.Select(otherLayer => ModuleLayerNamespace(otherModule, otherLayer))])
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Theory]
    [MemberData(nameof(AllModules))]
    public void IntegrationEvents_ShouldNotDependOn_AnyModuleLayer(string module)
    {
        var result = Types.InAssembly(ModuleLayer(module, IntegrationEventsLayer))
            .ShouldNot().HaveDependencyOnAny([.. AllBoundedContexts.SelectMany(anyModule =>
                ModuleLayers.Select(anyLayer => ModuleLayerNamespace(anyModule, anyLayer)))])
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
