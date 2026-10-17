using FluentValidation;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Validators;

public sealed class CompleteProductionStageCommandValidator : AbstractValidator<CompleteProductionStageCommand>
{
    public const int CompletedByMaxLength = 100;

    public CompleteProductionStageCommandValidator()
    {
        RuleFor(command => command.Stage).IsInEnum();
        RuleFor(command => command.CompletedBy).NotEmpty().MaximumLength(CompletedByMaxLength);
    }
}
