using System.Security.Claims;
using GeoNRage.Server.Models;
using GeoNRage.Server.Services;
using Gridify;
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

    private static async Task<Results<Ok<PaginationResult<ChallengeDto>>, ProblemHttpResult>> GetAllAsync(
        [AsParameters] PaginationQuery paginationQuery,
        ChallengeService challengeService,
        ClaimsPrincipal user,
        HttpContext httpContext,
        CancellationToken cancellationToken,
        [FromQuery] string[]? playersToExclude = null)
    {
        IGridifyMapper<ChallengeDto> mapper = new GridifyMapper<ChallengeDto>(true);

        if (!paginationQuery.IsValid(mapper))
        {
            return CustomTypedResults.Problem("Requête invalide");
        }

        IQueryable<ChallengeDto> query = challengeService.GetAll(
            httpContext.Request.Headers[Constants.MapStatusHeaderName] != "True",
            playersToExclude,
            user?.FindFirstValue("PlayerId"));

        return TypedResults.Ok(await query.PaginateAsync(paginationQuery, mapper, cancellationToken));
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

    private static async Task<Results<Ok<int>, ProblemHttpResult>> ImportChallengeAsync(ChallengeImportDto dto, ChallengeService challengeService)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            return TypedResults.Ok(await challengeService.ImportChallengeAsync(dto));
        }
        catch (Exception e) when (e is InvalidOperationException or HttpRequestException)
        {
            return CustomTypedResults.Problem(e.Message);
        }
    }

    private static async Task<NoContent> DeleteAsync(int id, ChallengeService challengeService)
    {
        await challengeService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
