using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class DeleteTrainingPlan
{
    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapDelete("/training-plans/{id:guid}", Handle)
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
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Training plan not found");
        context.TrainingPlans.Remove(trainingPlan);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(trainingPlan.Id));
    }
}