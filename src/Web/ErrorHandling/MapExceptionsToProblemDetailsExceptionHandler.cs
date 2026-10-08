using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RoboForge.Shared.Application.Exceptions;
using RoboForge.Shared.Domain;

namespace RoboForge.Web.ErrorHandling;

/// <summary>
/// Converts well-known exceptions into RFC 9457 <see cref="ProblemDetails"/> responses,
/// mapping <see cref="RequestValidationFailedException"/> → 400, <see cref="NotFoundException"/> → 404
/// and <see cref="DomainException"/> → 422 with the violated rule ID.
/// Unrecognised exceptions are not handled and fall through to the default middleware.
/// </summary>
public sealed class MapExceptionsToProblemDetailsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            RequestValidationFailedException ve => new ValidationProblemDetails(ve.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            NotFoundException ne => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Title = "The specified resource was not found.",
                Detail = ne.Message
            },
            DomainException de => new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.21",
                Title = "A business rule was violated.",
                Detail = $"{de.Rule}: {de.Message}",
                Extensions = { ["rules"] = new[] { de.Rule } }
            },
            _ => null
        };

        if (problemDetails is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
