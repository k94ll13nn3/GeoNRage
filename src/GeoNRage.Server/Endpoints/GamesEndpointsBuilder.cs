using GeoNRage.Server.Entities;
using GeoNRage.Server.Models;
using GeoNRage.Server.Services;
using Gridify;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Endpoints;

internal static class GamesEndpointsBuilder
{
    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/games").WithTags("Games");

        group.MapGet("/", GetAllAsync);
        group.MapGet("/admin-view", GetAllAsAdminViewAsync).RequireRole(Roles.Admin);
        group.MapGet("/{id}", GetAsync).WithName("GetById");

        group.MapPost("/", CreateAsync).RequireRole(Roles.Admin);
        group.MapPost("/{id}/add-player", AddPlayerAsync).RequireAuthorization();
        group.MapPost("/{id}/update-challenges", UpdateChallengesAsync).RequireRole(Roles.Admin);

        group.MapPut("/{id}", UpdateAsync).RequireRole(Roles.Admin);

        group.MapDelete("/{id}", DeleteAsync).RequireRole(Roles.Admin);

        return group;
    }

    private static async Task<Results<Ok<PaginationResult<GameDto>>, ProblemHttpResult>> GetAllAsync(
        [AsParameters] PaginationQuery paginationQuery,
        GameService gameService,
        CancellationToken cancellationToken)
    {
        IGridifyMapper<GameDto> mapper = new GridifyMapper<GameDto>(true);

        if (!paginationQuery.IsValid(mapper))
        {
            return CustomTypedResults.Problem("Requête invalide");
        }

        IQueryable<GameDto> query = gameService.GetAll();

        return TypedResults.Ok(await query.PaginateAsync(paginationQuery, mapper, cancellationToken));
    }

    private static async Task<Ok<IEnumerable<GameAdminViewDto>>> GetAllAsAdminViewAsync(GameService gameService)
    {
        return TypedResults.Ok(await gameService.GetAllAsAdminViewAsync());
    }

    private static async Task<Results<Ok<GameDetailDto>, NotFound>> GetAsync(int id, GameService gameService)
    {
        GameDetailDto? game = await gameService.GetAsync(id);
        return game is not null ? TypedResults.Ok(game) : TypedResults.NotFound();
    }

    private static async Task<Results<CreatedAtRoute, ProblemHttpResult>> CreateAsync(GameCreateOrEditDto dto, GameService gameService)
    {
        ArgumentNullException.ThrowIfNull(dto);
        try
        {
            int gameId = await gameService.CreateAsync(dto);
            return TypedResults.CreatedAtRoute("GetById", new { id = gameId });
        }
        catch (InvalidOperationException e)
        {
            return CustomTypedResults.Problem(e.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> UpdateAsync(int id, GameCreateOrEditDto dto, GameService gameService)
    {
        ArgumentNullException.ThrowIfNull(dto);
        try
        {
            Game? updatedGame = await gameService.UpdateAsync(id, dto);
            if (updatedGame is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return CustomTypedResults.Problem(e.Message);
        }
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> AddPlayerAsync(int id, [FromBody] string playerId, GameService gameService)
    {
        try
        {
            if (!gameService.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await gameService.AddPlayerAsync(id, playerId);

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return CustomTypedResults.Problem(e.Message);
        }
    }

    private static async Task<NoContent> DeleteAsync(int id, GameService gameService)
    {
        await gameService.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> UpdateChallengesAsync(int id, GameService gameService)
    {
        try
        {
            if (!gameService.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await gameService.ImportChallengeAsync(id);

            return TypedResults.NoContent();
        }
        catch (Exception e) when (e is InvalidOperationException or HttpRequestException)
        {
            return CustomTypedResults.Problem(e.Message);
        }
    }
}
