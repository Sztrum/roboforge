using FluentValidation;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Validators;

public sealed class ShipProductionJobCommandValidator : AbstractValidator<ShipProductionJobCommand>
{
    public const int TrackingNumberMaxLength = 100;

    public ShipProductionJobCommandValidator()
    {
        RuleFor(command => command.TrackingNumber).NotEmpty().MaximumLength(TrackingNumberMaxLength);
    }
}
