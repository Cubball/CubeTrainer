using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class GetTrainingPlan
{
    public sealed record CaseDto(
        Guid Id,
        string Name);

    public sealed record TrainingPlanCaseDto(
        int SolvesToLearnCount,
        string? LastDifficultyRating,
        DateTime? LastSolved,
        CaseDto Case);

    public sealed record TrainingPlanDto(Guid Id, string Name, List<TrainingPlanCaseDto> TrainingPlanCases);

    public sealed record Response(TrainingPlanDto TrainingPlan);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/training-plans/{id:guid}", Handle)
                .WithTags("Training Plans")
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
            .Include(static tp => tp.TrainingPlanCases)
            .ThenInclude(static tpc => tpc.Case)
            .AsSplitQuery()
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Training plan not found");
        var result = new TrainingPlanDto(
            trainingPlan.Id,
            trainingPlan.Name,
            trainingPlan.TrainingPlanCases.Select(tpc => new TrainingPlanCaseDto(
                tpc.SolvesToLearnCount,
                tpc.LastDifficultyRating?.ToString(),
                tpc.LastSolved,
                new(tpc.Case.Id, tpc.Case.Name)
            )).ToList());
        return Results.Ok(new Response(result));
    }
}