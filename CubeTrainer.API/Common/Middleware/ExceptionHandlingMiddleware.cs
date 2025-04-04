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
            _logger.LogWarning("Validation failed: {Message}", e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Bad request",
                errors = e.Errors.Select(static err => err.ErrorMessage)
            });
        }
        catch (UnauthorizedException e)
        {
            _logger.LogWarning("Unauthorized: {Message}", e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (ForbiddenException e)
        {
            _logger.LogWarning("Forbidden: {Message}", e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (NotFoundException e)
        {
            _logger.LogWarning("Not found: {Message}", e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal server error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
        }
    }
}