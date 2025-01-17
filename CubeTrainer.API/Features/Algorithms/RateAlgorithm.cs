using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class RateAlgorithm
{
    public sealed record Request(int? Rating);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Unless(
                static x => x.Rating is null,
                () => RuleFor(static x => x.Rating)
                        .GreaterThanOrEqualTo(1)
                        .LessThanOrEqualTo(5)
            );
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/algorithms/{id:guid}/rate", Handle)
                .WithTags("Algorithms")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        Request request,
        AppDbContext context,
        RequestValidator validator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var algorithm = await context.Algorithms
            .Where(a => a.Id == id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Algorithm not found");
        if (algorithm.IsDeleted || !algorithm.IsPublic)
        {
            throw new NotFoundException("Algorithm not found");
        }

        if (algorithm.CreatorId == userId)
        {
            throw new ForbiddenException("You are not allowed to rate your own algorithm");
        }

        var algorithmRating = await context.AlgorithmRatings
            .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.AlgorithmId == algorithm.Id, cancellationToken);
        if (algorithmRating is null && request.Rating is null)
        {
            throw new ValidationException([new ValidationFailure(nameof(request.Rating), "The rating value should be between 1 and 5 when the user hasn't rated the algorithm")]);
        }

        if (request.Rating is null)
        {
            algorithm.TotalRating = Math.Max(0, algorithm.TotalRating - algorithmRating!.Rating);
            algorithm.UsersRatingsCount = Math.Max(0, algorithm.UsersRatingsCount - 1);
            context.AlgorithmRatings.Remove(algorithmRating);
            await context.SaveChangesAsync(cancellationToken);
            return Results.Ok(new Response(algorithm.Id));
        }

        if (algorithmRating is null)
        {
            algorithmRating = new()
            {
                UserId = userId,
                AlgorithmId = algorithm.Id,
                Rating = request.Rating.Value,
            };
            context.AlgorithmRatings.Add(algorithmRating);
        }

        algorithmRating.Rating = request.Rating.Value;
        algorithm.UsersRatingsCount++;
        algorithm.TotalRating += request.Rating.Value;
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(algorithm.Id));
    }
}