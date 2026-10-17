using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Modules.Production.Domain.Jobs.Events;
using RoboForge.Modules.Production.Domain.Jobs.Exceptions;
using static RoboForge.Modules.Production.Domain.UnitTests.Jobs.ProductionJobAggregateExamples;

namespace RoboForge.Modules.Production.Domain.UnitTests.Jobs;

public class ProductionJobAggregateQaFailureTest
{
    [Fact]
    public void FailQa_FirstTime_SendsJobBackToAssemblyAndRaisesQaFailedEvent()
    {
        var job = JobAt(ProductionStageEnum.QaTests);

        job.FailQa("Luźne mocowanie napędu", StartedOnUtc);

        job.CurrentStage.ShouldBe(ProductionStageEnum.Assembly);
        job.Status.ShouldBe(ProductionJobStatusEnum.InProgress);
        var failedEvent = job.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<QaFailedEvent>();
        failedEvent.FailureCount.ShouldBe(1);
        failedEvent.IsOnHold.ShouldBeFalse();
    }

    [Fact]
    public void FailQa_SecondTime_PutsJobOnHold()
    {
        var job = JobAt(ProductionStageEnum.QaTests);
        job.FailQa("Pierwsza usterka", StartedOnUtc);
        job.CompleteStage(ProductionStageEnum.Assembly, Technician, StartedOnUtc);

        job.FailQa("Druga usterka", StartedOnUtc);

        job.Status.ShouldBe(ProductionJobStatusEnum.OnHold);
        job.CurrentStage.ShouldBe(ProductionStageEnum.Assembly);
        job.QaFailures.Count.ShouldBe(2);
        job.DomainEvents.OfType<QaFailedEvent>().Last().IsOnHold.ShouldBeTrue();
    }

    [Fact]
    public void CompleteStage_JobOnHold_ThrowsProductionJobIsOnHoldException()
    {
        var job = JobOnHold();

        var exception = Should.Throw<ProductionJobIsOnHoldException>(
            () => job.CompleteStage(ProductionStageEnum.Assembly, Technician, StartedOnUtc));

        exception.Rule.ShouldBe("R10");
    }

    [Fact]
    public void ReleaseFromHold_JobOnHold_LetsWorkContinueFromAssembly()
    {
        var job = JobOnHold();

        job.ReleaseFromHold();
        job.CompleteStage(ProductionStageEnum.Assembly, Technician, StartedOnUtc);

        job.CurrentStage.ShouldBe(ProductionStageEnum.QaTests);
    }

    [Fact]
    public void ReleaseFromHold_JobInProgress_ThrowsProductionJobIsNotOnHoldException()
    {
        var job = StartedJob();

        var exception = Should.Throw<ProductionJobIsNotOnHoldException>(job.ReleaseFromHold);

        exception.Rule.ShouldBe("P3");
    }

    [Fact]
    public void FailQa_OutsideQaStage_ThrowsQaCanFailOnlyDuringQaTestsException()
    {
        var job = JobAt(ProductionStageEnum.Assembly);

        var exception = Should.Throw<QaCanFailOnlyDuringQaTestsException>(() => job.FailQa("Usterka", StartedOnUtc));

        exception.Rule.ShouldBe("P2");
    }

    private static Domain.Jobs.ProductionJobAggregate JobOnHold()
    {
        var job = JobAt(ProductionStageEnum.QaTests);
        job.FailQa("Pierwsza usterka", StartedOnUtc);
        job.CompleteStage(ProductionStageEnum.Assembly, Technician, StartedOnUtc);
        job.FailQa("Druga usterka", StartedOnUtc);

        return job;
    }
}
