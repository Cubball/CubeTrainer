using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Solves;

internal static class ReportSolve
{
    public sealed record Request(Guid CaseId, decimal? Time);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Unless(
                static x => x.Time is null,
                () => RuleFor(static x => x.Time)
                        .GreaterThan(0)
            );
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/solves", Handle)
                .WithTags("Solves")
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
        var userCase = await context.UserCases
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(uc =>
                uc.UserId == userId
                && uc.CaseId == request.CaseId
                && uc.Status == UserCaseStatus.InProgress,
            cancellationToken)
            ?? throw new NotFoundException("Case not found");
        if (userCase.SelectedAlgorithm is null)
        {
            await context.SaveChangesAsync(cancellationToken);
            return Results.Ok(new Response(request.CaseId));
        }

        var algorithmStatistic = await context.AlgorithmStatistics
            .FirstOrDefaultAsync(@as => @as.UserId == userId && @as.AlgorithmId == userCase.SelectedAlgorithm.Id, cancellationToken);
        if (algorithmStatistic is null)
        {
            algorithmStatistic = new()
            {
                UserId = userId,
                AlgorithmId = userCase.SelectedAlgorithm.Id,
            };
            context.AlgorithmStatistics.Add(algorithmStatistic);
        }

        algorithmStatistic.BestTimeInSeconds = algorithmStatistic.BestTimeInSeconds is null
            ? request.Time
            : Math.Min(algorithmStatistic.BestTimeInSeconds.Value, request.Time ?? decimal.MaxValue);
        algorithmStatistic.TotalTimeSolvingInSeconds += request.Time ?? 0;
        algorithmStatistic.UntimedSolvesCount += request.Time is null ? 1 : 0;
        algorithmStatistic.TimedSolvesCount += request.Time is not null ? 1 : 0;
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(request.CaseId));
    }
}