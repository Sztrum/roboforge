using RoboForge.Modules.Catalog.Domain.Parts.Events;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.UnitTests.Parts;

public class PartAggregateDiscontinuationTest
{
    [Fact]
    public void Discontinue_ActivePart_MarksItDiscontinuedAndRaisesPartDiscontinuedEvent()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();
        drive.ClearDomainEvents();

        drive.Discontinue();

        drive.IsDiscontinued.ShouldBeTrue();
        var discontinuedEvent = drive.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<PartDiscontinuedEvent>();
        discontinuedEvent.PartId.ShouldBe(drive.Id);
    }

    [Fact]
    public void Discontinue_AlreadyDiscontinuedPart_ThrowsPartIsAlreadyDiscontinuedException()
    {
        var drive = PartAggregateExamples.CreateTrailDrive();
        drive.Discontinue();

        var exception = Should.Throw<PartIsAlreadyDiscontinuedException>(drive.Discontinue);

        exception.Rule.ShouldBe("C5");
    }
}
