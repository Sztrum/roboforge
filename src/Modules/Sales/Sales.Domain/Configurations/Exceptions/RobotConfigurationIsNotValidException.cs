using RoboForge.Shared.Domain.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Configurations.Exceptions;

/// <summary>
/// Thrown when an order is placed from a configuration that breaks rules R1–R4 or R7; carries every
/// violation, so the customer sees all problems at once.
/// </summary>
public sealed class RobotConfigurationIsNotValidException(IReadOnlyList<RuleViolation> violations)
    : DomainException(violations);
