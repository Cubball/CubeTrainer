namespace CubeTrainer.API.Common.Middleware;

internal sealed class ErrorHandlingMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        throw new NotImplementedException();
    }
}