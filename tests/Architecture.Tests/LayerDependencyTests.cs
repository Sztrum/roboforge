using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace RoboForge.Architecture.Tests;

public class LayerDependencyTests
{
    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Npgsql")]
    [InlineData("Microsoft.AspNetCore")]
    [InlineData("Mediator")]
    [InlineData("FluentValidation")]
    public void Domain_ShouldNotDependOn_Frameworks(string framework)
    {
        var result = Types.InAssembly(Layers.Domain)
            .ShouldNot().HaveDependencyOn(framework)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FailingTypes(result));
    }

    [Theory]
    [InlineData("RoboForge.Application")]
    [InlineData("RoboForge.Infrastructure")]
    [InlineData("RoboForge.Web")]
    public void Domain_ShouldNotDependOn_OuterLayers(string layer)
    {
        var result = Types.InAssembly(Layers.Domain)
            .ShouldNot().HaveDependencyOn(layer)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FailingTypes(result));
    }

    [Theory]
    [InlineData("RoboForge.Infrastructure")]
    [InlineData("RoboForge.Web")]
    [InlineData("Npgsql")]
    [InlineData("Microsoft.AspNetCore")]
    public void Application_ShouldNotDependOn_OuterLayers(string dependency)
    {
        var result = Types.InAssembly(Layers.Application)
            .ShouldNot().HaveDependencyOn(dependency)
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FailingTypes(result));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Web()
    {
        var result = Types.InAssembly(Layers.Infrastructure)
            .ShouldNot().HaveDependencyOn("RoboForge.Web")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(FailingTypes(result));
    }

    internal static string FailingTypes(TestResult result) =>
        "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
