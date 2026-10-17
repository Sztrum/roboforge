using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations.Enums;
using RoboForge.Modules.Sales.Domain.Parts;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Flags the discontinued part in every draft configuration that contains it, so their quotes report R7.</summary>
public sealed class MarkDraftConfigurationsInvalidWhenPartDiscontinuedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<PartDiscontinuedIntegrationEvent>
{
    public async ValueTask Handle(PartDiscontinuedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var partId = new CatalogPartId(notification.PartId);

        // Parts are stored as a JSON column, so the drafts are filtered after loading; fine for MVP volumes.
        var draftConfigurations = await dbContext.RobotConfigurations
            .Where(configuration => configuration.Status == RobotConfigurationStatusEnum.Draft)
            .ToListAsync(cancellationToken);

        foreach (var configuration in draftConfigurations.Where(draft => draft.Parts.Any(part => part.PartId == partId)))
        {
            configuration.MarkPartDiscontinued(partId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
