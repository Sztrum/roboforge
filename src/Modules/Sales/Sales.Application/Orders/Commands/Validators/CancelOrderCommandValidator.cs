using FluentValidation;

namespace RoboForge.Modules.Sales.Application.Orders.Commands.Validators;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public const int ReasonMaxLength = 500;

    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.Reason).NotEmpty().MaximumLength(ReasonMaxLength);
    }
}
