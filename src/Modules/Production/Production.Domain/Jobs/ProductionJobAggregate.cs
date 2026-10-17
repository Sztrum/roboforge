using System.Diagnostics.CodeAnalysis;
using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Modules.Production.Domain.Jobs.Events;
using RoboForge.Modules.Production.Domain.Jobs.Exceptions;
using RoboForge.Modules.Production.Domain.Jobs.ValueObjects;
using RoboForge.Shared.Domain.Aggregates;

namespace RoboForge.Modules.Production.Domain.Jobs;

/// <summary>
/// The workshop's job of building one ordered robot. It moves through the stages strictly in order
/// (R9); a failed QA sends it back to assembly, and after two failures it waits for a decision (R10).
/// </summary>
public sealed class ProductionJobAggregate : AggregateRoot<ProductionJobId>
{
    public const int QaFailuresBeforeHold = 2;

    private readonly List<ProductionPart> _parts;
    private readonly List<StageCompletion> _completedStages = [];
    private readonly List<QaFailure> _qaFailures = [];

    private ProductionJobAggregate(ProductionJobId id, SalesOrderId orderId, List<ProductionPart> parts, DateTime startedOnUtc)
        : base(id)
    {
        OrderId = orderId;
        _parts = parts;
        StartedOnUtc = startedOnUtc;
        CurrentStage = ProductionStageEnum.Printing;
        Status = ProductionJobStatusEnum.InProgress;
    }

    [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed",
        Justification = "EF Core calls it through reflection when loading a job.")]
    private ProductionJobAggregate(ProductionJobId id, SalesOrderId orderId, DateTime startedOnUtc)
        : base(id)
    {
        OrderId = orderId;
        StartedOnUtc = startedOnUtc;
        _parts = [];
    }

    public SalesOrderId OrderId { get; private set; }

    public ProductionStageEnum CurrentStage { get; private set; }

    public ProductionJobStatusEnum Status { get; private set; }

    public DateTime StartedOnUtc { get; private set; }

    public IReadOnlyList<ProductionPart> Parts => _parts.AsReadOnly();

    public IReadOnlyList<StageCompletion> CompletedStages => _completedStages.AsReadOnly();

    public IReadOnlyList<QaFailure> QaFailures => _qaFailures.AsReadOnly();

    public string? TrackingNumber { get; private set; }

    /// <summary>Starts building the robot of a confirmed order, beginning with printing the housing.</summary>
    public static ProductionJobAggregate StartForOrder(SalesOrderId orderId, IEnumerable<ProductionPart> parts, DateTime startedOnUtc) =>
        new(ProductionJobId.New(), orderId, [.. parts], startedOnUtc);

    /// <summary>Completes the current stage and moves to the next one (R9).</summary>
    public void CompleteStage(ProductionStageEnum stage, string completedBy, DateTime completedOnUtc)
    {
        GuardWorkCanContinue();

        if (CurrentStage == ProductionStageEnum.Shipped)
        {
            throw new ShippedStageIsCompletedByShippingException();
        }

        if (stage != CurrentStage)
        {
            throw new ProductionStageCannotBeSkippedException(CurrentStage, stage);
        }

        _completedStages.Add(new StageCompletion(stage, completedBy, completedOnUtc));
        CurrentStage = CurrentStage + 1;
        AddDomainEvent(new ProductionStageCompletedEvent(Id, OrderId, stage, completedBy, completedOnUtc));
    }

    /// <summary>Records a failed quality check: back to assembly, and on hold after the second failure (R10).</summary>
    public void FailQa(string reason, DateTime failedOnUtc)
    {
        GuardWorkCanContinue();

        if (CurrentStage != ProductionStageEnum.QaTests)
        {
            throw new QaCanFailOnlyDuringQaTestsException(CurrentStage);
        }

        _qaFailures.Add(new QaFailure(reason, failedOnUtc));
        CurrentStage = ProductionStageEnum.Assembly;

        if (_qaFailures.Count >= QaFailuresBeforeHold)
        {
            Status = ProductionJobStatusEnum.OnHold;
        }

        AddDomainEvent(new QaFailedEvent(Id, OrderId, reason, _qaFailures.Count, Status == ProductionJobStatusEnum.OnHold, failedOnUtc));
    }

    /// <summary>The workshop decided to continue a held job; it goes on from assembly.</summary>
    public void ReleaseFromHold()
    {
        if (Status != ProductionJobStatusEnum.OnHold)
        {
            throw new ProductionJobIsNotOnHoldException(Id);
        }

        Status = ProductionJobStatusEnum.InProgress;
    }

    /// <summary>Ships the packed robot; possible only after packing (R9).</summary>
    public void Ship(string trackingNumber, DateTime shippedOnUtc)
    {
        GuardWorkCanContinue();

        if (CurrentStage != ProductionStageEnum.Shipped)
        {
            throw new ProductionStageCannotBeSkippedException(CurrentStage, ProductionStageEnum.Shipped);
        }

        Status = ProductionJobStatusEnum.Shipped;
        TrackingNumber = trackingNumber;
        AddDomainEvent(new RobotShippedEvent(Id, OrderId, trackingNumber, shippedOnUtc));
    }

    private void GuardWorkCanContinue()
    {
        if (Status == ProductionJobStatusEnum.Shipped)
        {
            throw new ProductionJobIsAlreadyShippedException(Id);
        }

        if (Status == ProductionJobStatusEnum.OnHold)
        {
            throw new ProductionJobIsOnHoldException(Id);
        }
    }
}
