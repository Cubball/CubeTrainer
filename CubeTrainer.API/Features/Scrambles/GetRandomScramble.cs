using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using CubeTrainer.Cube;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Scrambles;

internal static class GetRandomScramble
{
    public sealed record CaseDto(
        Guid Id,
        string Name,
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
        if (userCases.Count == 0)
        {
            throw new NotFoundException("No cases in progress found");
        }

        var randomCase = userCases[Random.Shared.Next(userCases.Count)];

        var scramble = GetRandomScrambleForCase(randomCase.Case.DefaultSolution);

        var result = new ScrambleDto(scramble, new(
            randomCase.Case.Id,
            randomCase.Case.Name,
            randomCase.SelectedAlgorithm is null
                ? null
                : new(randomCase.SelectedAlgorithm.Id, randomCase.SelectedAlgorithm.Moves)
        ));
        return Results.Ok(new Response(result));
    }

    private static string GetRandomScrambleForCase(string caseSolution)
    {
        var caseScramble = MoveSequence.FromString(caseSolution).Inverse();
        var setupMoves = MoveSequence.Random(Random.Shared.Next(1, 4));
        var cube = RubiksCube.Scrambled(caseScramble.Append(setupMoves));
        var cubeScramble = RubiksCubeSolver.FindSolution(cube).Inverse();
        var scramble = cubeScramble.Append(setupMoves.Inverse());
        return scramble.ToString();
    }
}