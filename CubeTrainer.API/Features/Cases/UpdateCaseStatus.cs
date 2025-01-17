using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using CubeTrainer.API.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Cases;

internal static class UpdateCaseStatus
{
    public sealed record Request(string Status);

    public sealed class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(static x => x.Status)
                .IsEnumName(typeof(UserCaseStatus));
        }
    }

    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapPut("/cases/{id:guid}", Handle)
                .WithTags("Cases")
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
        if (!Enum.TryParse<UserCaseStatus>(request.Status, out var status))
        {
            throw new ValidationException([new ValidationFailure(nameof(request.Status), "Status is not valid")]);
        }

        var userId = (user.Claims.FirstOrDefault(static c => c.Type == Constants.Auth.UserIdClaimType)?.Value)
            ?? throw new UnauthorizedException("User not found");
        var userCase = await context.UserCases
            .Where(uc => uc.UserId == userId && uc.CaseId == id)
            .Include(uc => uc.Case)
            .FirstOrDefaultAsync(cancellationToken);
        if (userCase is not null)
        {
            userCase.Status = status;
            await context.SaveChangesAsync(cancellationToken);
            return Results.Ok(new Response(id));
        }

        var @case = await context.Cases
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new UnauthorizedException("Case not found");
        userCase = new()
        {
            CaseId = id,
            UserId = userId,
            Status = status,
        };
        context.UserCases.Add(userCase);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(id));
    }
}