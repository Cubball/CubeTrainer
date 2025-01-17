using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class SearchAlgorithms
{
    public sealed record CaseDto(
        Guid Id,
        string Type,
        string Name,
        string ImageUrl);

    public sealed record AlgorithmDto(
        Guid Id,
        CaseDto Case,
        string Moves,
        bool IsPublic,
        // TODO: IsMine
        DateTime CreatedAt,
        int UsersCount,
        int TotalRating,
        int UsersRatingsCount);

    public sealed record Response(List<AlgorithmDto> Algorithms);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                // TODO: pagination
                .MapGet("/algorithms/cases/{caseId:guid}", Handle)
                .WithTags("Algorithms")
                // NOTE: might make some routes, like this one, public
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid caseId,
        AppDbContext context,
        CancellationToken cancellationToken,
        string sortBy = "users",
        bool ascending = false)
    {
        var query = context.Algorithms
            .Include(a => a.Case)
            // NOTE: this query will load the user's own public algorithms too, not sure if we want that
            .Where(a => a.IsPublic && !a.IsDeleted && a.CaseId == caseId);
        query = sortBy switch
        {
            "rating" => ascending
                ? query.OrderBy(a => a.TotalRating / Math.Max(a.UsersRatingsCount, 1))
                : query.OrderByDescending(a => a.TotalRating / Math.Max(a.UsersRatingsCount, 1)),
            "created" => ascending
                ? query.OrderBy(a => a.CreatedAt)
                : query.OrderByDescending(a => a.CreatedAt),
            _ => ascending
                ? query.OrderBy(a => a.UsersCount)
                : query.OrderByDescending(a => a.UsersCount),
        };
        var algorithms = await query
            .Select(a => new AlgorithmDto(
                a.Id,
                new CaseDto(
                    a.Case.Id,
                    a.Case.Type.ToString(),
                    a.Case.Name,
                    a.Case.ImageUrl),
                a.Moves,
                a.IsPublic,
                a.CreatedAt,
                a.UsersCount,
                a.TotalRating,
                a.UsersRatingsCount))
            .ToListAsync(cancellationToken);
        return Results.Ok(new Response(algorithms));
    }
}