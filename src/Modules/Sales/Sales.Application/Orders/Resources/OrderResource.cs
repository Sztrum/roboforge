using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Orders.Enums;

namespace RoboForge.Modules.Sales.Application.Orders.Resources;

/// <summary>An order with its frozen parts and price, and the production progress so far.</summary>
public sealed record OrderResource(
    Guid Id,
    Guid ConfigurationId,
    OrderStatusEnum Status,
    IReadOnlyList<PartSnapshotResource> Parts,
    decimal PartsPriceAmount,
    decimal AssemblyPriceAmount,
    decimal DiscountAmount,
    decimal TotalPriceAmount,
    string Currency,
    int LeadTimeWorkingDays,
    DateTime PlacedOnUtc,
    IReadOnlyList<OrderStageEntryResource> CompletedStages,
    IReadOnlyList<OrderNoteResource> Notes,
    string? TrackingNumber,
    string? CancellationReason)
{
    public static OrderResource FromAggregate(OrderAggregate order) => new(
        order.Id.Value,
        order.ConfigurationId.Value,
        order.Status,
        [.. order.Parts.Select(PartSnapshotResource.FromSnapshot)],
        order.Price.PartsPrice.Amount,
        order.Price.AssemblyPrice.Amount,
        order.Price.Discount.Amount,
        order.Price.TotalPrice.Amount,
        order.Price.TotalPrice.Currency,
        order.LeadTime.WorkingDays,
        order.PlacedOnUtc,
        [.. order.CompletedStages.Select(entry => new OrderStageEntryResource(entry.Stage, entry.CompletedOnUtc))],
        [.. order.Notes.Select(note => new OrderNoteResource(note.Text, note.AddedOnUtc))],
        order.TrackingNumber,
        order.CancellationReason);
}
