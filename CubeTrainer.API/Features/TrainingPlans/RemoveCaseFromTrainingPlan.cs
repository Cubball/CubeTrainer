using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class RemoveCaseFromTrainingPlan
{
    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapDelete("/training-plans/{id:guid}/cases/{caseId:guid}", Handle)
                .WithTags("Training Plans")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        Guid caseId,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var trainingPlanCase = await context.TrainingPlanCases
            .Include(tpc => tpc.TrainingPlan)
            .FirstOrDefaultAsync(tpc => tpc.TrainingPlanId == id && tpc.CaseId == caseId, cancellationToken)
            ?? throw new NotFoundException("Case not found");
        if (trainingPlanCase.TrainingPlan.UserId != userId)
        {
            throw new NotFoundException("Case not found");
        }

        context.TrainingPlanCases.Remove(trainingPlanCase);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(id));
    }
}