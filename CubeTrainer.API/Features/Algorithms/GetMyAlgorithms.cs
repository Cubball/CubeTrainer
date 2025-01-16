using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class GetMyAlgorithms
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
        DateTime CreatedAt,
        int UsersCount,
        int StarsCount,
        int UsersRatingsCount);

    public sealed record Response(List<AlgorithmDto> Algorithms);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/algorithms/{type:regex((OLL|PLL))}/my", Handle)
                .WithTags("Algorithms")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        string type,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<CaseType>(type, out var parsedType))
        {
            throw new ValidationException([new ValidationFailure(nameof(type), "Type is not valid")]);
        }

        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var algorithms = await context.Algorithms
            .Include(a => a.Case)
            .Where(a => a.CreatorId == userId
                && a.Case.Type == parsedType
                && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
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
                a.StarsCount,
                a.UsersRatingsCount))
            .ToListAsync(cancellationToken);
        return Results.Ok(new Response(algorithms));
    }
}