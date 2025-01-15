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
                message = e.Message,
                errors = e.Errors.Select(static err => err.ErrorMessage)
            });
        }
        catch (UnauthorizedException e)
        {
            _logger.LogError(e, "Unauthorized error");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (NotFoundException e)
        {
            _logger.LogError(e, "Not found error");
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Internal server error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
    }
}