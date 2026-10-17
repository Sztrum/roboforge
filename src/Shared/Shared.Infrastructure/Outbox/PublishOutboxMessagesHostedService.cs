using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RoboForge.Shared.Infrastructure.Outbox;

/// <summary>
/// Polls one module's outbox and publishes each waiting integration event in-process through Mediator,
/// so the other modules' handlers run. Replaceable by a message broker without touching the domain.
/// </summary>
public sealed partial class PublishOutboxMessagesHostedService<TDbContext>(
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider timeProvider,
    ILogger<PublishOutboxMessagesHostedService<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    private const int MessagesPerBatch = 20;

    private const int PollingIntervalMilliseconds = 500;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var pollingTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(PollingIntervalMilliseconds), timeProvider);

        while (await pollingTimer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PublishWaitingMessagesAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // A failed round (e.g. the database restarting) must not stop the publisher; the next tick retries.
                LogPublishingRoundFailed(exception, typeof(TDbContext).Name);
            }
        }
    }

    private async Task PublishWaitingMessagesAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var waitingMessages = await dbContext.Set<OutboxMessage>()
            .Where(message => message.ProcessedOnUtc == null)
            .OrderBy(message => message.OccurredOnUtc)
            .Take(MessagesPerBatch)
            .ToListAsync(cancellationToken);

        foreach (var message in waitingMessages)
        {
            await PublishMessageAsync(publisher, message, cancellationToken);
        }

        if (waitingMessages.Count != 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task PublishMessageAsync(IPublisher publisher, OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await publisher.Publish(message.ToIntegrationEvent(), cancellationToken);
            message.MarkAsPublished(timeProvider.GetUtcNow().UtcDateTime);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            LogMessagePublishingFailed(exception, message.Id, message.Type);
            message.MarkAsFailed(timeProvider.GetUtcNow().UtcDateTime, exception.ToString());
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Publishing outbox messages of {DbContextName} failed")]
    private partial void LogPublishingRoundFailed(Exception exception, string dbContextName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Outbox message {MessageId} of type {MessageType} could not be published")]
    private partial void LogMessagePublishingFailed(Exception exception, Guid messageId, string messageType);
}
