using Microsoft.AspNetCore.Mvc;
using RoboForge.Shared.Application.Behaviours;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiHostServiceRegistration
{
    /// <summary>
    /// Registers what every module shares in the API host: the Mediator request pipeline,
    /// exception-to-ProblemDetails mapping and the OpenAPI document.
    /// </summary>
    public static void AddApiHostServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediator((MediatorOptions options) =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors =
            [
                typeof(LogRequestPipelineBehavior<,>),
                typeof(ValidateRequestPipelineBehavior<,>),
                typeof(WarnAboutSlowRequestPipelineBehavior<,>),
            ];
        });

        builder.Services.AddExceptionHandler<MapExceptionsToProblemDetailsExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
            options.AddOperationTransformer<AddErrorResponsesOpenApiOperationTransformer>());
    }
}
