using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Cases;

internal static class GetMyCases
{
    public sealed record CaseDto(
        Guid Id,
        string Name,
        string ImageUrl,
        AlgorithmDto? SelectedAlgorithm);

    public sealed record AlgorithmDto(
        Guid Id,
        string Moves);

    public sealed record Response(List<CaseDto> Items);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/cases/{type:regex((OLL|PLL))}/my", Handle)
                .WithTags("Cases")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        string type,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        if (!Enum.TryParse<CaseType>(type, out var parsedType))
        {
            throw new ValidationException([new ValidationFailure(nameof(type), "Type is not valid")]);
        }

        var cases = await context.Cases
            .Where(c => c.Type == parsedType)
            .ToListAsync(cancellationToken);
        var userCases = await context.UserCases
            .Where(uc => uc.UserId == userId)
            .Include(uc => uc.SelectedAlgorithm)
            .ToListAsync(cancellationToken);
        var result = new List<CaseDto>();
        foreach (var @case in cases)
        {
            var userCase = userCases.FirstOrDefault(uc => uc.CaseId == @case.Id);
            if (userCase is null || userCase.SelectedAlgorithm is null)
            {
                result.Add(new(@case.Id, @case.Name, @case.ImageUrl, null));
                continue;
            }

            result.Add(
                new(
                    @case.Id,
                    @case.Name,
                    @case.ImageUrl,
                    new(
                        userCase.SelectedAlgorithm.Id,
                        userCase.SelectedAlgorithm.Moves
                    )
                )
            );
        }

        return Results.Ok(new Response(result));
    }
}