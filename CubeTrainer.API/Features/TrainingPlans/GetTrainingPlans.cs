using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class GetTrainingPlans
{
    public sealed record TrainingPlanDto(Guid Id, string Name);

    public sealed record Response(List<TrainingPlanDto> Items);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapGet("/training-plans", Handle)
                .WithTags("Training Plans")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var trainingPlans = await context.TrainingPlans
            .Where(tp => tp.UserId == userId)
            .Select(tp => new TrainingPlanDto(tp.Id, tp.Name))
            .ToListAsync(cancellationToken);
        return Results.Ok(new Response(trainingPlans));
    }
}