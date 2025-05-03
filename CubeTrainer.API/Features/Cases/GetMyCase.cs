using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using CubeTrainer.Cube;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Cases;

internal static class GetMyCase
{
    public sealed record CaseDto(
        Guid Id,
        string Name,
        string ImageUrl,
        string Status,
        string DefaultScramble,
        AlgorithmDto? SelectedAlgorithm);

    public sealed record AlgorithmRatingDto(int Rating);

    public sealed record AlgorithmStatisticDto(
        decimal TotalTimeSolvingInSeconds,
        int TimedSolvesCount,
        int UntimedSolvesCount,
        decimal? BestTimeInSeconds);

    public sealed record AlgorithmDto(
        Guid Id,
        string Moves,
        bool IsPublic,
        bool IsMine,
        DateTime CreatedAt,
        int UsersCount,
        int TotalRating,
        int UsersRatingsCount,
        AlgorithmRatingDto? MyRating,
        AlgorithmStatisticDto MyStatistic);

    public sealed record Response(CaseDto Case);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/cases/{id:guid}", Handle)
                .WithTags("Cases")
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
        var userCase = await context.UserCases
            .Include(uc => uc.Case)
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CaseId == id, cancellationToken);
        if (userCase is null)
        {
            var @case = await context.Cases.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw new NotFoundException("Case not found");
            return Results.Ok(new Response(
                new(
                    @case.Id,
                    @case.Name,
                    @case.ImageUrl,
                    UserCaseStatus.NotLearned.ToString(),
                    MoveSequence.FromString(@case.DefaultSolution).Inverse().ToString(),
                    null
                )));
        }

        if (userCase.SelectedAlgorithm is null)
        {
            return Results.Ok(new Response(
                new(
                    userCase.Case.Id,
                    userCase.Case.Name,
                    userCase.Case.ImageUrl,
                    userCase.Status.ToString(),
                    MoveSequence.FromString(userCase.Case.DefaultSolution).Inverse().ToString(),
                    null))
            );
        }

        var algorithmStatistic = await context.AlgorithmStatistics
            .FirstOrDefaultAsync(@as => @as.UserId == userId && @as.AlgorithmId == userCase.SelectedAlgorithm.Id, cancellationToken);
        var algorithmRating = await context.AlgorithmRatings
            .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.AlgorithmId == userCase.SelectedAlgorithm.Id, cancellationToken);
        var result = new CaseDto(
            userCase.Case.Id,
            userCase.Case.Name,
            userCase.Case.ImageUrl,
            userCase.Status.ToString(),
            MoveSequence.FromString(userCase.Case.DefaultSolution).Inverse().ToString(),
            new(
                userCase.SelectedAlgorithm.Id,
                userCase.SelectedAlgorithm.Moves,
                userCase.SelectedAlgorithm.IsPublic,
                userCase.SelectedAlgorithm.CreatorId == userId,
                userCase.SelectedAlgorithm.CreatedAt,
                userCase.SelectedAlgorithm.UsersCount,
                userCase.SelectedAlgorithm.TotalRating,
                userCase.SelectedAlgorithm.UsersRatingsCount,
                algorithmRating is null ? null : new(algorithmRating.Rating),
                new(
                    algorithmStatistic?.TotalTimeSolvingInSeconds ?? 0,
                    algorithmStatistic?.TimedSolvesCount ?? 0,
                    algorithmStatistic?.UntimedSolvesCount ?? 0,
                    algorithmStatistic?.BestTimeInSeconds))
        );
        return Results.Ok(new Response(result));
    }
}