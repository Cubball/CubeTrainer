using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class SelectAlgorithm
{
    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/algorithms/{id:guid}/select", Handle)
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
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Algorithm not found");
        if (algorithm.IsDeleted || (!algorithm.IsPublic && algorithm.CreatorId != userId))
        {
            throw new NotFoundException("Algorithm not found");
        }

        var userCase = await context.UserCases
            .Where(uc => uc.UserId == userId && uc.CaseId == algorithm.CaseId)
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(cancellationToken);
        if (userCase is null)
        {
            userCase = new UserCase
            {
                UserId = userId,
                CaseId = algorithm.CaseId,
                SelectedAlgorithmId = algorithm.Id,
                Status = UserCaseStatus.NotLearned,
            };
            context.UserCases.Add(userCase);
        }
        else
        {
            userCase.SelectedAlgorithmId = algorithm.Id;
            if (userCase.SelectedAlgorithm is not null)
            {
                userCase.SelectedAlgorithm.UsersCount = Math.Max(0, userCase.SelectedAlgorithm.UsersCount - 1);
            }
        }

        var algorithmStatistic = await context.AlgorithmStatistics
            .Where(@as => @as.UserId == userId && @as.AlgorithmId == algorithm.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (algorithmStatistic is null)
        {
            algorithmStatistic = new()
            {
                UserId = userId,
                AlgorithmId = algorithm.Id,
            };
            context.AlgorithmStatistics.Add(algorithmStatistic);
        }

        algorithm.UsersCount++;
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(algorithm.Id));
    }
}