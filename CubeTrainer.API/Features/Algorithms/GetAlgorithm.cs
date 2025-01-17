using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class GetAlgorithm
{
    public sealed record CaseDto(
        Guid Id,
        string Type,
        string Name,
        string ImageUrl);

    public sealed record AlgorithmRatingDto(int? Rating);

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
        CaseDto Case,
        AlgorithmRatingDto? MyRating,
        AlgorithmStatisticDto MyStatistic);

    public sealed record Response(AlgorithmDto Algorithm);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/algorithms/{id:guid}", Handle)
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
            .Include(a => a.Case)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
            ?? throw new NotFoundException("Algorithm not found");
        if (algorithm.IsDeleted || (!algorithm.IsPublic && algorithm.CreatorId != userId))
        {
            throw new NotFoundException("Algorithm not found");
        }

        var algorithmStatistic = await context.AlgorithmStatistics
            .FirstOrDefaultAsync(@as => @as.UserId == userId && @as.AlgorithmId == id, cancellationToken);
        var algorithmRating = await context.AlgorithmRatings
            .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.AlgorithmId == id, cancellationToken);
        var result = new AlgorithmDto(
            algorithm.Id,
            algorithm.Moves,
            algorithm.IsPublic,
            algorithm.CreatorId == userId,
            algorithm.CreatedAt,
            algorithm.UsersCount,
            algorithm.TotalRating,
            algorithm.UsersRatingsCount,
            new(algorithm.Case.Id, algorithm.Case.Type.ToString(), algorithm.Case.Name, algorithm.Case.ImageUrl),
            algorithmRating is null ? null : new(algorithmRating.Rating),
            new(
                algorithmStatistic?.TotalTimeSolvingInSeconds ?? 0,
                algorithmStatistic?.TimedSolvesCount ?? 0,
                algorithmStatistic?.UntimedSolvesCount ?? 0,
                algorithmStatistic?.BestTimeInSeconds));
        return Results.Ok(new Response(result));
    }
}