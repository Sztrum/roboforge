using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Configurations.Exceptions;

/// <summary>Thrown when a part that is not in a configuration is removed from it (rule S2).</summary>
public sealed class PartIsNotInRobotConfigurationException(CatalogPartId partId)
    : DomainException("S2", $"Części {partId.Value} nie ma w tej konfiguracji.");
