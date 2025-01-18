using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class AddCaseToTrainingPlan
{
    public sealed record Request(Guid CaseId);

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/training-plans/{id:guid}/cases", Handle)
                .WithTags("Training Plans")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        Request request,
        AppDbContext context,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var trainingPlan = await context.TrainingPlans
            .FirstOrDefaultAsync(tp => tp.Id == id && tp.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Training plan not found");
        var trainingPlanCase = await context.TrainingPlanCases
            .FirstOrDefaultAsync(tpc => tpc.TrainingPlanId == id && tpc.CaseId == request.CaseId, cancellationToken);
        if (trainingPlanCase is not null)
        {
            throw new ValidationException([new ValidationFailure(nameof(request.CaseId), "The case is already in the training plan")]);
        }

        var @case = await context.Cases
            .FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
            ?? throw new NotFoundException("Case not found");
        trainingPlanCase = new TrainingPlanCase
        {
            TrainingPlanId = id,
            CaseId = request.CaseId,
            SolvesToLearnCount = 3,
        };
        context.TrainingPlanCases.Add(trainingPlanCase);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(id));
    }
}