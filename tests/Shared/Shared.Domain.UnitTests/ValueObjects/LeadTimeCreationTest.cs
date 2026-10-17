using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class LeadTimeCreationTest
{
    [Fact]
    public void Constructor_WithOneWorkingDay_IsAllowed()
    {
        var shortestLeadTime = new LeadTime(1);

        shortestLeadTime.WorkingDays.ShouldBe(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_WithLessThanOneWorkingDay_ThrowsLeadTimeMustBeAtLeastOneWorkingDayException(int invalidWorkingDays)
    {
        var exception = Should.Throw<LeadTimeMustBeAtLeastOneWorkingDayException>(() => new LeadTime(invalidWorkingDays));

        exception.Rule.ShouldBe("L1");
    }

    [Fact]
    public void Equality_WithSameWorkingDays_IsTrue()
    {
        new LeadTime(5).ShouldBe(new LeadTime(5));
    }
}
