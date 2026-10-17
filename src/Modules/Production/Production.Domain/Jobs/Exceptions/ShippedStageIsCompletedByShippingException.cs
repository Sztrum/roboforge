using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when the shipping stage is completed without shipping the robot with a tracking number (rule P1).</summary>
public sealed class ShippedStageIsCompletedByShippingException()
    : DomainException("P1", "Etap wysyłki kończy się nadaniem paczki z numerem przesyłki.");
