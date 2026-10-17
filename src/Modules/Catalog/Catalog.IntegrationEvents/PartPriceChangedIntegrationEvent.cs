using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Catalog.IntegrationEvents;

/// <summary>A catalog part got a new price; placed orders keep the price from their snapshot.</summary>
public sealed record PartPriceChangedIntegrationEvent(Guid PartId, decimal PriceAmount, string PriceCurrency) : IntegrationEvent;
