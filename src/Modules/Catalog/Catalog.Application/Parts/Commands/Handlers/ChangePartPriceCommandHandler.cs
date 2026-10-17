using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands.Handlers;

public sealed class ChangePartPriceCommandHandler(ICatalogDbContext dbContext) : ICommandHandler<ChangePartPriceCommand, PartResource>
{
    public async ValueTask<PartResource> Handle(ChangePartPriceCommand command, CancellationToken cancellationToken)
    {
        var part = await dbContext.Parts.FindAsync([new PartId(command.PartId)], cancellationToken);
        Guard.Against.NotFound(command.PartId, part);

        part.ChangePrice(new Money(command.PriceAmount, Money.DefaultCurrency));
        await dbContext.SaveChangesAsync(cancellationToken);

        return PartResource.FromAggregate(part);
    }
}
