using Microsoft.Extensions.Logging;

namespace RoboForge.Shared.Application.Behaviours;

public sealed partial class LogRequestPipelineBehavior<TMessage, TResponse>(ILogger<LogRequestPipelineBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        LogRequest(typeof(TMessage).Name, message);

        return next(message, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "RoboForge Request: {Name} {@Request}")]
    private partial void LogRequest(string name, TMessage request);
}
