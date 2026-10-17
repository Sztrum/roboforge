using RoboForge.Modules.Sales.Domain.Orders.Enums;

namespace RoboForge.Modules.Sales.Application.Orders.Resources;

public sealed record OrderStageEntryResource(ProductionStageEnum Stage, DateTime CompletedOnUtc);
