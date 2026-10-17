using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Configurations.Exceptions;
using RoboForge.Modules.Sales.Domain.Parts;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Configurations;

public class RobotConfigurationAggregatePartChangesTest
{
    [Fact]
    public void AddPart_OfferedPart_AddsItsSnapshot()
    {
        var configuration = RobotConfigurationAggregate.Create();
        var camera = Camera();

        configuration.AddPart(camera);

        configuration.Parts.ShouldHaveSingleItem().ShouldBe(camera);
    }

    [Fact]
    public void AddPart_DiscontinuedPart_ThrowsDiscontinuedPartCannotBeAddedException()
    {
        var configuration = RobotConfigurationAggregate.Create();

        var exception = Should.Throw<DiscontinuedPartCannotBeAddedException>(
            () => configuration.AddPart(Camera().AsDiscontinued()));

        exception.Rule.ShouldBe("R7");
    }

    [Fact]
    public void RemovePart_PartAddedTwice_RemovesOnePiece()
    {
        var configuration = RobotConfigurationAggregate.Create();
        var camera = Camera();
        configuration.AddPart(camera);
        configuration.AddPart(camera);

        configuration.RemovePart(camera.PartId);

        configuration.Parts.ShouldHaveSingleItem();
    }

    [Fact]
    public void RemovePart_PartNotInConfiguration_ThrowsPartIsNotInRobotConfigurationException()
    {
        var configuration = RobotConfigurationAggregate.Create();

        var exception = Should.Throw<PartIsNotInRobotConfigurationException>(
            () => configuration.RemovePart(new CatalogPartId(Guid.NewGuid())));

        exception.Rule.ShouldBe("S2");
    }

    [Fact]
    public void MarkPartDiscontinued_PartInDraft_IsReportedByValidateAsR7()
    {
        var configuration = RobotConfigurationAggregate.Create();
        var robotParts = CompatibleRangerRobot();
        foreach (var part in robotParts)
        {
            configuration.AddPart(part);
        }

        var discontinuedCamera = robotParts[^1];
        configuration.MarkPartDiscontinued(discontinuedCamera.PartId);

        configuration.Validate().ShouldHaveSingleItem().Rule.ShouldBe("R7");
    }
}
