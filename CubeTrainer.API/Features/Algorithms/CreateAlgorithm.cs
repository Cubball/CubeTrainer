using System.Security.Claims;
using System.Text;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Common.Helpers;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Algorithms;

internal static class CreateAlgorithm
{
    // TODO: how to handle guid not parsed
    public sealed record Request(Guid CaseId, string Moves);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(static x => x.CaseId)
                .NotEmpty();
            RuleFor(static x => x.Moves)
                .NotEmpty()
                .MaximumLength(500);
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            // TODO: add more swagger info
            builder
                .MapPost("/algorithms", Handle)
                .WithTags("Algorithms")
                // .RequireAuthorization()
                ;
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
        var (normalizedMoves, isValid) = TryNormalizeMoves(request.Moves);
        if (!isValid)
        {
            throw new ValidationException([new ValidationFailure(nameof(request.Moves), "Invalid moves")]);
        }

        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var @case = await context.Cases.FirstOrDefaultAsync(c => c.Id == request.CaseId, cancellationToken)
            ?? throw new ValidationException([new ValidationFailure(nameof(request.CaseId), "Case not found")]);
        var algorithm = new Algorithm
        {
            Id = Guid.NewGuid(),
            CaseId = request.CaseId,
            Moves = normalizedMoves,
            CreatorId = userId,
            CreatedAt = DateTimeHelpers.UtcNow,
        };
        context.Algorithms.Add(algorithm);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(algorithm.Id));
    }

    private static (string, bool) TryNormalizeMoves(string moves)
    {
        const string validMoves = "UDLRFBMESxyzudlrfb";
        const char counterClockwise = '\'';
        const char doubleTurn = '2';
        var sb = new StringBuilder();
        var previousIsMove = false;
        for (var i = 0; i < moves.Length; i++)
        {
            var move = moves[i];
            if (char.IsWhiteSpace(move))
            {
                continue;
            }

            var isMove = validMoves.Contains(move);
            if (!isMove && !previousIsMove)
            {
                return (string.Empty, false);
            }

            if (isMove)
            {
                sb.Append(' ');
                sb.Append(move);
                previousIsMove = true;
                continue;
            }

            if (move == counterClockwise)
            {
                sb.Append(move);
                previousIsMove = false;
                continue;
            }

            if (move == doubleTurn)
            {
                sb.Append(move);
                previousIsMove = false;
                continue;
            }

            return (string.Empty, false);
        }

        return (sb.ToString().Trim(), true);
    }
}