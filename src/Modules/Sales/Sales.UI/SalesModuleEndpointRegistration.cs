using Microsoft.AspNetCore.Builder;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Sales.UI;

public static class SalesModuleEndpointRegistration
{
    /// <summary>Maps every endpoint group of the Sales module (<c>/api/configurations</c>, <c>/api/orders</c>).</summary>
    public static WebApplication MapSalesModuleEndpoints(this WebApplication app) =>
        app.MapEndpointGroups(typeof(SalesModuleEndpointRegistration).Assembly);
}
