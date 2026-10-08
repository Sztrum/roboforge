using Microsoft.Extensions.Logging;

namespace RoboForge.Application.Common.Behaviours;

public sealed class LoggingBehaviour<TMessage, TResponse>(ILogger<TMessage> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("RoboForge Request: {Name} {@Request}", typeof(TMessage).Name, message);

        return next(message, cancellationToken);
    }
}
