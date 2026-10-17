using Mediator;
using RoboForge.Modules.Catalog.Application.Parts.Resources;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands;

/// <summary>Sets a new price (in PLN) for a part that is still offered.</summary>
public sealed record ChangePartPriceCommand(Guid PartId, decimal PriceAmount) : ICommand<PartResource>;
