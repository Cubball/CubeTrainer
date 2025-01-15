using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class DeleteAlgorithm
{
    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapDelete("/algorithms/{id}", Handle)
                .WithTags("Algorithms")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var algorithm = await context.Algorithms
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Algorithm not found");
        if (algorithm.CreatorId != userId)
        {
            throw new ForbiddenException("You are not the creator of this algorithm");
        }

        if (algorithm.IsDeleted)
        {
            throw new ForbiddenException("Algorithm is already deleted");
        }

        algorithm.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(algorithm.Id));
    }
}