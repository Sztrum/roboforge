using Microsoft.Extensions.Logging;

namespace RoboForge.Application.Common.Behaviours;

public sealed class UnhandledExceptionBehaviour<TMessage, TResponse>(ILogger<TMessage> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "RoboForge Request: Unhandled Exception for Request {Name} {@Request}", typeof(TMessage).Name, message);

            throw;
        }
    }
}
