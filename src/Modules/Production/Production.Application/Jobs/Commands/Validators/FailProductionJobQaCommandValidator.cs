using FluentValidation;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Validators;

public sealed class FailProductionJobQaCommandValidator : AbstractValidator<FailProductionJobQaCommand>
{
    public const int ReasonMaxLength = 500;

    public FailProductionJobQaCommandValidator()
    {
        RuleFor(command => command.Reason).NotEmpty().MaximumLength(ReasonMaxLength);
    }
}
