using System.Diagnostics.CodeAnalysis;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;
using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Modules.Sales.Domain.Orders.Events;
using RoboForge.Modules.Sales.Domain.Orders.Exceptions;
using RoboForge.Modules.Sales.Domain.Orders.ValueObjects;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Aggregates;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Orders;

/// <summary>
/// A customer's order for a robot. It keeps its own copy of the parts, price and lead time from the
/// moment it was placed, so later catalog changes do not affect it; production progress reaches it
/// only through events from the Production module.
/// </summary>
public sealed class OrderAggregate : AggregateRoot<OrderId>
{
    /// <summary>A failed QA sends the robot back to assembly, so the order needs assembly and QA again (rule S6).</summary>
    public const int WorkingDaysAddedByFailedQa = LeadTime.AssemblyWorkingDays + LeadTime.QaWorkingDays;

    private readonly List<PartSnapshot> _parts;
    private readonly List<OrderStageEntry> _completedStages = [];
    private readonly List<OrderNote> _notes = [];

    private OrderAggregate(
        OrderId id,
        RobotConfigurationId configurationId,
        List<PartSnapshot> parts,
        RobotPrice price,
        LeadTime leadTime,
        DateTime placedOnUtc)
        : base(id)
    {
        ConfigurationId = configurationId;
        _parts = parts;
        Price = price;
        LeadTime = leadTime;
        PlacedOnUtc = placedOnUtc;
        Status = OrderStatusEnum.Placed;
    }

    [SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed",
        Justification = "EF Core calls it through reflection when loading an order, then sets the value objects.")]
    private OrderAggregate(OrderId id, RobotConfigurationId configurationId, DateTime placedOnUtc)
        : base(id)
    {
        ConfigurationId = configurationId;
        PlacedOnUtc = placedOnUtc;
        _parts = [];
        Price = null!;
        LeadTime = null!;
    }

    public RobotConfigurationId ConfigurationId { get; private set; }

    public OrderStatusEnum Status { get; private set; }

    public IReadOnlyList<PartSnapshot> Parts => _parts.AsReadOnly();

    public RobotPrice Price { get; private set; }

    public LeadTime LeadTime { get; private set; }

    public DateTime PlacedOnUtc { get; private set; }

    public IReadOnlyList<OrderStageEntry> CompletedStages => _completedStages.AsReadOnly();

    public IReadOnlyList<OrderNote> Notes => _notes.AsReadOnly();

    public string? TrackingNumber { get; private set; }

    public string? CancellationReason { get; private set; }

    /// <summary>
    /// Places an order from a valid configuration and locks the configuration; an invalid one is refused
    /// with every violation at once (R1–R4, R7).
    /// </summary>
    public static OrderAggregate Place(RobotConfigurationAggregate configuration, DateTime placedOnUtc)
    {
        configuration.MarkAsOrdered();

        var quote = configuration.PrepareQuote();

        return new OrderAggregate(OrderId.New(), configuration.Id, [.. configuration.Parts], quote.Price, quote.LeadTime, placedOnUtc);
    }

    /// <summary>Confirms the order, which starts production.</summary>
    public void Confirm()
    {
        if (Status != OrderStatusEnum.Placed)
        {
            throw new OrderCanBeConfirmedOnlyWhenPlacedException(Id, Status);
        }

        Status = OrderStatusEnum.Confirmed;
        AddDomainEvent(new OrderConfirmedEvent(Id, Parts));
    }

    /// <summary>Cancels the order; possible only before production starts (R8).</summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatusEnum.Cancelled)
        {
            throw new OrderIsAlreadyCancelledException(Id);
        }

        if (Status != OrderStatusEnum.Placed)
        {
            throw new OrderCannotBeCancelledInProductionException(Id, Status);
        }

        Status = OrderStatusEnum.Cancelled;
        CancellationReason = reason;
        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    public void MarkStageReached(ProductionStageEnum stage, DateTime completedOnUtc)
    {
        GuardIsBeingProduced();

        Status = OrderStatusEnum.InProduction;
        _completedStages.Add(new OrderStageEntry(stage, completedOnUtc));
    }

    /// <summary>Tells the customer about a failed quality check and extends the lead time (S6).</summary>
    public void RecordFailedQa(string reason, DateTime failedOnUtc)
    {
        GuardIsBeingProduced();

        _notes.Add(new OrderNote($"Kontrola jakości nie powiodła się: {reason}. Robot wraca do montażu.", failedOnUtc));
        LeadTime = LeadTime.ExtendBy(WorkingDaysAddedByFailedQa);
    }

    public void MarkShipped(string trackingNumber, DateTime shippedOnUtc)
    {
        GuardIsBeingProduced();

        Status = OrderStatusEnum.Shipped;
        TrackingNumber = trackingNumber;
        _completedStages.Add(new OrderStageEntry(ProductionStageEnum.Shipped, shippedOnUtc));
    }

    private void GuardIsBeingProduced()
    {
        if (Status is not (OrderStatusEnum.Confirmed or OrderStatusEnum.InProduction))
        {
            throw new OrderIsNotInProductionException(Id, Status);
        }
    }
}
