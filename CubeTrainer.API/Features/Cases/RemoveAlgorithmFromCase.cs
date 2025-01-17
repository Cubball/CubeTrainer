using System.Security.Claims;
using CubeTrainer.API.Common;
using CubeTrainer.API.Common.Endpoints;
using CubeTrainer.API.Common.Exceptions;
using CubeTrainer.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Features.Cases;

internal static class RemoveAlgorithmFromCase
{
    public sealed record Response(Guid Id);

    public sealed class Endpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder builder)
        {
            builder
                .MapDelete("/cases/{id:guid}/algorithm", Handle)
                .WithTags("Cases")
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
        var userCase = await context.UserCases
            .Where(uc => uc.UserId == userId && uc.CaseId == id)
            .Include(uc => uc.SelectedAlgorithm)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("User's case not found");
        if (userCase.SelectedAlgorithm is null)
        {
            return Results.Ok(new Response(id));
        }

        userCase.SelectedAlgorithmId = null;
        userCase.SelectedAlgorithm.UsersCount = Math.Max(0, userCase.SelectedAlgorithm.UsersCount - 1);
        await context.SaveChangesAsync(cancellationToken);
        return Results.Ok(new Response(id));
    }
}