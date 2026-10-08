using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebLayerServiceRegistration
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<MapExceptionsToProblemDetailsExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
            options.AddOperationTransformer<AddErrorResponsesOpenApiOperationTransformer>());
    }
}
