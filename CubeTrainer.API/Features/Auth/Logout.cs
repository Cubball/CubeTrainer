using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CubeTrainer.API.Features.Auth;

internal static class Logout
{
    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/logout", Handle)
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        SignInManager<User> signInManager,
        [FromBody] object? empty)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
}