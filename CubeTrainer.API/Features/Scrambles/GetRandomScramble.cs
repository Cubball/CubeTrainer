using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Scrambles;

internal static class GetRandomScramble
{
    public sealed record CaseDto(
        Guid Id,
        string Name,
        string ImageUrl,
        AlgorithmDto? SelectedAlgorithm);

    public sealed record AlgorithmDto(
        Guid Id,
        string Moves);

    public sealed record ScrambleDto(string Moves, CaseDto Case);

    public sealed record Response(ScrambleDto Scramble);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/scrambles/random", Handle)
                .WithTags("Scrambles")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var userCases = await context.UserCases
            .Where(uc => uc.UserId == userId && uc.Status == UserCaseStatus.InProgress)
            .Include(uc => uc.Case)
            .Include(uc => uc.SelectedAlgorithm)
            .ToListAsync(cancellationToken);
        var randomCase = userCases[Random.Shared.Next(userCases.Count)];

        // TODO: use Kociemba to gen random scrable for randomCase
        // add a few random moves before feeding it to it, to ensure randomness
        // after that reverse both Kociemba and setup moves
        var scramble = "R U R' U'";

        var result = new ScrambleDto(scramble, new(
            randomCase.Case.Id,
            randomCase.Case.Name,
            randomCase.Case.ImageUrl,
            randomCase.SelectedAlgorithm is null
                ? null
                : new(randomCase.SelectedAlgorithm.Id, randomCase.SelectedAlgorithm.Moves)
        ));
        return Results.Ok(new Response(result));
    }
}