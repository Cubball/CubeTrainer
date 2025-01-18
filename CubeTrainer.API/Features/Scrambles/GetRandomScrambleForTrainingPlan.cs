using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Scrambles;

internal static class GetRandomScrambleForTrainingPlan
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
                .MapGet("/scrambles/training-plans/{id:guid}/random", Handle)
                .WithTags("Scrambles")
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
        var trainingPlan = await context.TrainingPlans
            .Include(tp => tp.TrainingPlanCases.Where(tpc => tpc.SolvesToLearnCount > 0))
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Training plan not found");
        if (trainingPlan.TrainingPlanCases.Count == 0)
        {
            throw new NotFoundException("No cases in progress found");
        }

        var randomCase = trainingPlan.TrainingPlanCases[Random.Shared.Next(trainingPlan.TrainingPlanCases.Count)];

        // TODO: use Kociemba to gen random scrable for randomCase
        // add a few random moves before feeding it to it, to ensure randomness
        // after that reverse both Kociemba and setup moves
        var scramble = "R U R' U'";

        var @case = await context.Cases
            .FirstAsync(c => c.Id == randomCase.CaseId, cancellationToken);
        var userCase = await context.UserCases
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CaseId == randomCase.CaseId, cancellationToken);
        var result = new ScrambleDto(scramble, new(
            @case.Id,
            @case.Name,
            @case.ImageUrl,
            userCase?.SelectedAlgorithm is null
                ? null
                : new(userCase.SelectedAlgorithm.Id, userCase.SelectedAlgorithm.Moves)
        ));
        return Results.Ok(new Response(result));
    }
}