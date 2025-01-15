using System.Net;
using CubeTrainer.API.Common.Exceptions;
using FluentValidation;

namespace CubeTrainer.API.Common.Middleware;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            _logger.LogError(e, "Validation error");
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Bad request",
                errors = e.Errors.Select(static err => err.ErrorMessage)
            });
        }
        catch (UnauthorizedException e)
        {
            _logger.LogError(e, "Unauthorized error");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
        }
        catch (ForbiddenException e)
        {
            _logger.LogError(e, "Forbidden error");
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Forbidden" });
        }
        catch (NotFoundException e)
        {
            _logger.LogError(e, "Not found error");
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new { message = "Not found" });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal server error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
        }
    }
}