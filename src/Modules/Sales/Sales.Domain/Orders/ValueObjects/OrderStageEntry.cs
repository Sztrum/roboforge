using RoboForge.Modules.Sales.Domain.Orders.Enums;

namespace RoboForge.Modules.Sales.Domain.Orders.ValueObjects;

/// <summary>A production stage the order's robot completed, and when.</summary>
public sealed record OrderStageEntry(ProductionStageEnum Stage, DateTime CompletedOnUtc);
