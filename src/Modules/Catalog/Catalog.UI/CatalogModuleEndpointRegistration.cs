using Microsoft.AspNetCore.Builder;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Catalog.UI;

public static class CatalogModuleEndpointRegistration
{
    /// <summary>Maps every endpoint group of the Catalog module (<c>/api/parts</c>).</summary>
    public static WebApplication MapCatalogModuleEndpoints(this WebApplication app) =>
        app.MapEndpointGroups(typeof(CatalogModuleEndpointRegistration).Assembly);
}
