using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RoboForge.Application.Common.Behaviours;

public sealed class PerformanceBehaviour<TMessage, TResponse>(ILogger<TMessage> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    private const long SlowRequestThresholdMilliseconds = 500;

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var startedAt = Stopwatch.GetTimestamp();

        var response = await next(message, cancellationToken);

        var elapsedMilliseconds = (long)Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;

        if (elapsedMilliseconds > SlowRequestThresholdMilliseconds)
        {
            logger.LogWarning(
                "RoboForge Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                typeof(TMessage).Name, elapsedMilliseconds, message);
        }

        return response;
    }
}
