using NetArchTest.Rules;
using static RoboForge.Architecture.Tests.ArchitectureTestResultMessage;
using static RoboForge.Architecture.Tests.ProjectAssemblies;

namespace RoboForge.Architecture.Tests;

/// <summary>
/// Inside every module, dependencies point inward: UI → Infrastructure → Application → Domain.
/// </summary>
public class ModuleLayerDependencyArchitectureTest
{
    private static readonly string[] Frameworks =
        ["Microsoft.EntityFrameworkCore", "Npgsql", "Microsoft.AspNetCore", "Mediator", "FluentValidation"];

    public static TheoryData<string, string> ModulesWithFrameworks() =>
        new(ExistingModules.SelectMany(module => Frameworks.Select(framework => (module, framework))));

    public static TheoryData<string> AllModules() => new(ExistingModules);

    [Theory]
    [MemberData(nameof(ModulesWithFrameworks))]
    public void Domain_ShouldNotDependOn_Frameworks(string module, string framework)
    {
        var result = Types.InAssembly(ModuleLayer(module, "Domain"))
            .ShouldNot().HaveDependencyOn(framework)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Theory]
    [MemberData(nameof(AllModules))]
    public void Domain_ShouldNotDependOn_OuterLayersOfItsModule(string module)
    {
        var result = Types.InAssembly(ModuleLayer(module, "Domain"))
            .ShouldNot().HaveDependencyOnAny(
                ModuleLayerNamespace(module, "Application"),
                ModuleLayerNamespace(module, "Infrastructure"),
                ModuleLayerNamespace(module, "UI"))
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Theory]
    [MemberData(nameof(AllModules))]
    public void Application_ShouldNotDependOn_InfrastructureOrUi(string module)
    {
        var result = Types.InAssembly(ModuleLayer(module, "Application"))
            .ShouldNot().HaveDependencyOnAny(
                ModuleLayerNamespace(module, "Infrastructure"),
                ModuleLayerNamespace(module, "UI"),
                "Npgsql",
                "Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }

    [Theory]
    [MemberData(nameof(AllModules))]
    public void Infrastructure_ShouldNotDependOn_Ui(string module)
    {
        var result = Types.InAssembly(ModuleLayer(module, "Infrastructure"))
            .ShouldNot().HaveDependencyOn(ModuleLayerNamespace(module, "UI"))
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(ListViolatingTypes(result));
    }
}
