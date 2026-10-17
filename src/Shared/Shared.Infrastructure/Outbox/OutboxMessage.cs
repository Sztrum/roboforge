using System.Text.Json;
using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Shared.Infrastructure.Outbox;

/// <summary>
/// An integration event saved in the same transaction as the change that raised it, waiting to be
/// published by <see cref="PublishOutboxMessagesHostedService{TDbContext}"/>.
/// </summary>
public sealed class OutboxMessage
{
    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; private set; }

    /// <summary>The event's type as "Namespace.TypeName, AssemblyName", enough to load it again.</summary>
    public string Type { get; private set; }

    public string Content { get; private set; }

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage FromIntegrationEvent(IntegrationEvent integrationEvent)
    {
        var eventType = integrationEvent.GetType();

        return new OutboxMessage(
            integrationEvent.Id,
            $"{eventType.FullName}, {eventType.Assembly.GetName().Name}",
            JsonSerializer.Serialize(integrationEvent, eventType),
            integrationEvent.OccurredOnUtc);
    }

    public IntegrationEvent ToIntegrationEvent()
    {
        var eventType = System.Type.GetType(Type, throwOnError: true)!;

        return (IntegrationEvent)JsonSerializer.Deserialize(Content, eventType)!;
    }

    public void MarkAsPublished(DateTime publishedOnUtc) => ProcessedOnUtc = publishedOnUtc;

    /// <summary>Records why publishing failed; the message is not retried, so the error stays visible.</summary>
    public void MarkAsFailed(DateTime failedOnUtc, string error)
    {
        ProcessedOnUtc = failedOnUtc;
        Error = error;
    }
}
