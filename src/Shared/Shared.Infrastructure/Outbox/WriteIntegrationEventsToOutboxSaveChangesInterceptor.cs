using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoboForge.Shared.Domain.Aggregates;
using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Shared.Infrastructure.Outbox;

/// <summary>
/// Before saving, takes the domain events of every tracked aggregate, turns the ones other modules care
/// about into integration events and adds them to the outbox, so they are stored in the same transaction
/// as the change (docs/PROJECT.md, "Mechanics").
/// </summary>
/// <param name="mapToIntegrationEvent">The module's translation; returns null for events that stay inside the module.</param>
public sealed class WriteIntegrationEventsToOutboxSaveChangesInterceptor(Func<IDomainEvent, IntegrationEvent?> mapToIntegrationEvent)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        WriteToOutbox(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        WriteToOutbox(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void WriteToOutbox(DbContext? dbContext)
    {
        if (dbContext is null)
        {
            return;
        }

        var aggregatesWithEvents = dbContext.ChangeTracker.Entries<IAggregateRoot>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count != 0)
            .ToList();

        var outboxMessages = aggregatesWithEvents
            .SelectMany(aggregate => aggregate.DomainEvents)
            .Select(mapToIntegrationEvent)
            .OfType<IntegrationEvent>()
            .Select(OutboxMessage.FromIntegrationEvent)
            .ToList();

        aggregatesWithEvents.ForEach(aggregate => aggregate.ClearDomainEvents());

        dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
