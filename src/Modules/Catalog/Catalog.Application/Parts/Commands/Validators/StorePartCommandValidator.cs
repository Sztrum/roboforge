using FluentValidation;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands.Validators;

/// <summary>Checks the request's shape; business rules (C1–C3, M1, L1) are checked by the domain.</summary>
public sealed class StorePartCommandValidator : AbstractValidator<StorePartCommand>
{
    public const int NameMaxLength = 200;

    public StorePartCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(NameMaxLength);
        RuleFor(command => command.Category).IsInEnum();
        RuleFor(command => command.MountType).IsInEnum();
    }
}
