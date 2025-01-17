using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;

namespace CubeTrainer.API.Features.TrainingPlans;

internal static class CreateTrainingPlan
{
    public sealed record Request(string Name);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(static x => x.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/training-plans", Handle)
                .WithTags("Training Plans")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Request request,
        AppDbContext context,
        RequestValidator validator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var trainingPlan = new TrainingPlan
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = userId,
        };
        context.TrainingPlans.Add(trainingPlan);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(trainingPlan.Id));
    }
}