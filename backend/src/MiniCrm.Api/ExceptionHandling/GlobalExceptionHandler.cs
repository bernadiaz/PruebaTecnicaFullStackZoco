using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniCrm.Application.Exceptions;

namespace MiniCrm.Api.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail, errors) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso no encontrado", e.Message, null),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflicto", e.Message, null),
            BusinessValidationException e => (StatusCodes.Status400BadRequest, "Solicitud inválida", e.Message, e.Errors),
            _ => (StatusCodes.Status500InternalServerError, "Error interno", "Ocurrió un error inesperado.", null)
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (errors is { Count: > 0 })
        {
            problem.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
