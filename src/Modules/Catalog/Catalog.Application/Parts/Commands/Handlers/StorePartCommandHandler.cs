using Mediator;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands.Handlers;

public sealed class StorePartCommandHandler(ICatalogDbContext dbContext) : ICommandHandler<StorePartCommand, PartResource>
{
    public async ValueTask<PartResource> Handle(StorePartCommand command, CancellationToken cancellationToken)
    {
        var part = PartAggregate.Create(
            command.Name,
            command.Category,
            new Money(command.PriceAmount, Money.DefaultCurrency),
            new LeadTime(command.LeadTimeWorkingDays),
            new TechSpec(command.WeightGrams, command.MountType, command.VoltageVolts, command.MaxLoadGrams));

        dbContext.Parts.Add(part);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PartResource.FromAggregate(part);
    }
}
