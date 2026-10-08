using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace RoboForge.Web.Infrastructure;

/// <summary>
/// Adds standard error responses to every OpenAPI operation: 400 Bad Request, because every
/// request passes through <c>ValidationBehaviour</c> in the Mediator pipeline, and
/// 422 Unprocessable Entity, because any command can break a business rule.
/// </summary>
internal sealed class ApiExceptionOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Responses ??= [];
        operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Bad Request" });
        operation.Responses.TryAdd("422", new OpenApiResponse { Description = "Business rule violated" });

        return Task.CompletedTask;
    }
}
