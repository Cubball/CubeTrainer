using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Common.Helpers;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Solves;

internal static class ReportSolveForTrainingPlan
{
    // NOTE: might tweak these
    private const int NumberOfSolvesReductionAfterEasy = 1;
    private const int NumberOfSolvesIncreaseAfterHard = 2;
    private const int MaxNumberOfSolves = 20;
    private const double TimeMultiplierPerDay = .1;

    public sealed record Request(Guid CaseId, decimal Time, string DifficultyRating);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(static x => x.Time)
                .GreaterThan(0);
            RuleFor(static x => x.DifficultyRating)
                .IsEnumName(typeof(DifficultyRating));
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/solves/training-plans/{id:guid}", Handle)
                .WithTags("Solves")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        Request request,
        AppDbContext context,
        RequestValidator validator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        if (!Enum.TryParse<DifficultyRating>(request.DifficultyRating, out var difficultyRating))
        {
            throw new ValidationException([new ValidationFailure(nameof(request.DifficultyRating), "Difficulty rating is not valid")]);
        }

        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var trainingPlanCase = await context.TrainingPlanCases
            .FirstOrDefaultAsync(tpc => tpc.TrainingPlanId == id && tpc.CaseId == request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case not found");
        var userCase = await context.UserCases
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CaseId == request.CaseId, cancellationToken);
        var solvesDeltaFromDifficulty = difficultyRating switch
        {
            DifficultyRating.Easy => -NumberOfSolvesReductionAfterEasy,
            DifficultyRating.Hard => NumberOfSolvesIncreaseAfterHard,
            DifficultyRating.Normal => 0,
            _ => 0,
        };
        var now = DateTimeHelpers.UtcNow;
        var solvesDeltaFromTime = trainingPlanCase.LastSolved is null
            ? 0
            : now.Subtract(trainingPlanCase.LastSolved.Value).Days * TimeMultiplierPerDay;
        var newSolvesToLearnCount = trainingPlanCase.SolvesToLearnCount
            + solvesDeltaFromDifficulty
            + solvesDeltaFromTime;
        newSolvesToLearnCount = Math.Ceiling(Math.Clamp(newSolvesToLearnCount, 0, MaxNumberOfSolves));
        trainingPlanCase.LastSolved = now;
        trainingPlanCase.LastDifficultyRating = difficultyRating;
        trainingPlanCase.SolvesToLearnCount = (int)newSolvesToLearnCount;

        if (userCase?.SelectedAlgorithm is null)
        {
            await context.SaveChangesAsync(cancellationToken);
            return Results.Ok(new Response(request.CaseId));
        }

        var algorithmStatistic = await context.AlgorithmStatistics
            .FirstOrDefaultAsync(@as => @as.UserId == userId && @as.AlgorithmId == userCase.SelectedAlgorithm.Id, cancellationToken);
        if (algorithmStatistic is null)
        {
            algorithmStatistic = new()
            {
                UserId = userId,
                AlgorithmId = userCase.SelectedAlgorithm.Id,
            };
            context.AlgorithmStatistics.Add(algorithmStatistic);
        }

        algorithmStatistic.BestTimeInSeconds = algorithmStatistic.BestTimeInSeconds is null
            ? request.Time
            : Math.Min(algorithmStatistic.BestTimeInSeconds.Value, request.Time);
        algorithmStatistic.TotalTimeSolvingInSeconds += request.Time;
        algorithmStatistic.TimedSolvesCount++;
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(request.CaseId));
    }
}