using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Common.Models;
using CubeTrainer.API.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class SearchAlgorithms
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 5;

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
        bool IsMine,
        DateTime CreatedAt,
        int UsersCount,
        int TotalRating,
        int UsersRatingsCount);

    public sealed record Response(PagedList<AlgorithmDto> Algorithms);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/algorithms/cases/{caseId:guid}", Handle)
                .WithTags("Algorithms")
                // NOTE: might make some routes, like this one, public
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid caseId,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken,
        int page = DefaultPageNumber,
        int pageSize = DefaultPageSize,
        string sortBy = "users",
        bool ascending = false)
    {
        page = Math.Max(page, DefaultPageNumber);
        pageSize = pageSize > 0 ? pageSize : DefaultPageSize;
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var query = context.Algorithms
            .Include(a => a.Case)
            // NOTE: this query will load the user's own public algorithms too, not sure if we want that
            .Where(a => a.IsPublic && !a.IsDeleted && a.CaseId == caseId);
        var totalCount = await query.CountAsync(cancellationToken);
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
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(a => new AlgorithmDto(
                a.Id,
                new CaseDto(
                    a.Case.Id,
                    a.Case.Type.ToString(),
                    a.Case.Name,
                    a.Case.ImageUrl),
                a.Moves,
                a.IsPublic,
                a.CreatorId == userId,
                a.CreatedAt,
                a.UsersCount,
                a.TotalRating,
                a.UsersRatingsCount))
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var pagedList = new PagedList<AlgorithmDto>(page, pageSize, totalPages, totalCount, algorithms);
        return Results.Ok(new Response(pagedList));
    }
}