using Mediator;
using RoboForge.Modules.Catalog.Application.Parts.Resources;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands;

/// <summary>Withdraws a part from the offer for good.</summary>
public sealed record DiscontinuePartCommand(Guid PartId) : ICommand<PartResource>;
