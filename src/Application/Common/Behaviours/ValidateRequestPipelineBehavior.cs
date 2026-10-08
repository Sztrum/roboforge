using RoboForge.Application.Common.Exceptions;

namespace RoboForge.Application.Common.Behaviours;

public sealed class ValidateRequestPipelineBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TMessage>(message), cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                throw new RequestValidationFailedException(failures);
            }
        }

        return await next(message, cancellationToken);
    }
}
