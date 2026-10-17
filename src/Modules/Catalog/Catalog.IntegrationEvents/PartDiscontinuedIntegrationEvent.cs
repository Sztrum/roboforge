using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Catalog.IntegrationEvents;

/// <summary>A part was withdrawn from the offer for good.</summary>
public sealed record PartDiscontinuedIntegrationEvent(Guid PartId) : IntegrationEvent;
