using System.Security.Claims;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Endpoints;

internal static class ChallengesEndpointsBuilder
{
    public static RouteGroupBuilder MapChallengesEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/challenges").RequireRole(Roles.Member).WithTags("Challenges");

        group.MapGet("/", GetAllAsync);
        group.MapGet("/admin-view", GetAllAsAdminViewAsync).RequireRole(Roles.Admin);
        group.MapGet("/{id}", GetAsync);

        group.MapPost("/import", ImportChallengeAsync);

        group.MapDelete("/{id}", DeleteAsync).RequireRole(Roles.Admin);

        return group;
    }

    private static async Task<Ok<IEnumerable<ChallengeDto>>> GetAllAsync(
        ChallengeService challengeService,
        ClaimsPrincipal user,
        HttpContext httpContext,
        bool onlyWithoutGame = false,
        [FromQuery] string[]? playersToExclude = null)
    {
        return TypedResults.Ok(await challengeService.GetAllAsync(
            onlyWithoutGame,
            httpContext.Request.Headers[Constants.MapStatusHeaderName] != "True",
            playersToExclude,
            user?.FindFirstValue("PlayerId")));
    }

    private static async Task<Ok<IEnumerable<ChallengeAdminViewDto>>> GetAllAsAdminViewAsync(ChallengeService challengeService)
    {
        return TypedResults.Ok(await challengeService.GetAllAsAdminViewAsync());
    }

    private static async Task<Results<Ok<ChallengeDetailDto>, NotFound>> GetAsync(int id, ChallengeService challengeService)
    {
        if (await challengeService.GetAsync(id) is ChallengeDetailDto challenge)
        {
            return TypedResults.Ok(challenge);
        }

        return TypedResults.NotFound();
    }

    private static async Task<Results<Ok<int>, BadRequest<ApiError>>> ImportChallengeAsync(ChallengeImportDto dto, ChallengeService challengeService)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));

        try
        {
            return TypedResults.Ok(await challengeService.ImportChallengeAsync(dto));
        }
        catch (HttpRequestException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }

    private static async Task<NoContent> DeleteAsync(int id, ChallengeService challengeService)
    {
        await challengeService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
