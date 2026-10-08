using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RoboForge.Application.Common.Behaviours;

public sealed partial class PerformanceBehaviour<TMessage, TResponse>(ILogger<PerformanceBehaviour<TMessage, TResponse>> logger)
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
            LogSlowRequest(typeof(TMessage).Name, elapsedMilliseconds, message);
        }

        return response;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "RoboForge Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}")]
    private partial void LogSlowRequest(string name, long elapsedMilliseconds, TMessage request);
}
