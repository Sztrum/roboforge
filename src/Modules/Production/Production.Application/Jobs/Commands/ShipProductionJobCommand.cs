using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;

namespace RoboForge.Modules.Production.Application.Jobs.Commands;

/// <summary>Ships the packed robot with a tracking number.</summary>
public sealed record ShipProductionJobCommand(Guid JobId, string TrackingNumber) : ICommand<ProductionJobResource>;
