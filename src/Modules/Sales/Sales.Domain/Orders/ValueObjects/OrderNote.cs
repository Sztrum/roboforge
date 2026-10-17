namespace RoboForge.Modules.Sales.Domain.Orders.ValueObjects;

/// <summary>Something the customer should know about the order, e.g. a failed quality check.</summary>
public sealed record OrderNote(string Text, DateTime AddedOnUtc);
