using NetArchTest.Rules;

namespace RoboForge.Architecture.Tests;

/// <summary>
/// Bounded contexts (Catalog, Sales, Production) must not use each other's domain model;
/// they integrate only through events (docs/PROJECT.md, "Domain events and integration").
/// </summary>
public class BoundedContextTests
{
    [Theory]
    [InlineData("Catalog", "Sales")]
    [InlineData("Catalog", "Production")]
    [InlineData("Sales", "Catalog")]
    [InlineData("Sales", "Production")]
    [InlineData("Production", "Catalog")]
    [InlineData("Production", "Sales")]
    public void DomainContext_ShouldNotDependOn_OtherContext(string context, string otherContext)
    {
        var result = Types.InAssembly(Layers.Domain)
            .That().ResideInNamespace($"RoboForge.Domain.{context}")
            .ShouldNot().HaveDependencyOn($"RoboForge.Domain.{otherContext}")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(LayerDependencyTests.FailingTypes(result));
    }

    [Theory]
    [InlineData("Catalog", "Sales")]
    [InlineData("Catalog", "Production")]
    [InlineData("Sales", "Catalog")]
    [InlineData("Sales", "Production")]
    [InlineData("Production", "Catalog")]
    [InlineData("Production", "Sales")]
    public void ApplicationContext_ShouldNotDependOn_OtherContext(string context, string otherContext)
    {
        var result = Types.InAssembly(Layers.Application)
            .That().ResideInNamespace($"RoboForge.Application.{context}")
            .ShouldNot().HaveDependencyOn($"RoboForge.Application.{otherContext}")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(LayerDependencyTests.FailingTypes(result));
    }
}
