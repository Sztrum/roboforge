using Microsoft.AspNetCore.Builder;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Production.UI;

public static class ProductionModuleEndpointRegistration
{
    /// <summary>Maps every endpoint group of the Production module (<c>/api/production/jobs</c>).</summary>
    public static WebApplication MapProductionModuleEndpoints(this WebApplication app) =>
        app.MapEndpointGroups(typeof(ProductionModuleEndpointRegistration).Assembly);
}
