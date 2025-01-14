using System.Security.Claims;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class CreateAlgorithm
{
    public record Request(Guid CaseId, string Moves);

    public record Response(Guid Id);

    public class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/algorithms", Handle)
                .WithTags("Algorithms")
                .RequireAuthorization();
        }
    }

    // TODO: error handling
    private static async Task<IResult> Handle(
        Request request,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var @case = await context.Cases.FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken);
        if (@case is null)
        {
            // TODO:
            return Results.NotFound("Case not found.");
        }

        var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        // TODO: Validate moves
        var algorithm = new Algorithm
        {
            Id = Guid.NewGuid(),
            CaseId = request.CaseId,
            Moves = request.Moves,
            CreatorId = userId,
            CreatedAt = DateTime.UtcNow,
        };

        context.Algorithms.Add(algorithm);
        await context.SaveChangesAsync(cancellationToken);
        // TODO:
        return Results.Ok(new Response(algorithm.Id));
    }
}