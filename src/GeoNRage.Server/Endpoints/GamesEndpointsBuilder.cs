using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Endpoints;

public static class GamesEndpointsBuilder
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

    private static async Task<Ok<IEnumerable<GameDto>>> GetAllAsync(GameService _gameService)
    {
        return TypedResults.Ok(await _gameService.GetAllAsync());
    }

    private static async Task<Ok<IEnumerable<GameAdminViewDto>>> GetAllAsAdminViewAsync(GameService _gameService)
    {
        return TypedResults.Ok(await _gameService.GetAllAsAdminViewAsync());
    }

    private static async Task<Results<Ok<GameDetailDto>, NotFound>> GetAsync(int id, GameService _gameService)
    {
        GameDetailDto? game = await _gameService.GetAsync(id);
        return game is not null ? TypedResults.Ok(game) : TypedResults.NotFound();
    }

    private static async Task<Results<CreatedAtRoute, BadRequest<ApiError>>> CreateAsync(GameCreateOrEditDto dto, GameService _gameService)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            int gameId = await _gameService.CreateAsync(dto);
            return TypedResults.CreatedAtRoute("GetById", new { id = gameId });
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> UpdateAsync(int id, GameCreateOrEditDto dto, GameService _gameService)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            Game? updatedGame = await _gameService.UpdateAsync(id, dto);
            if (updatedGame is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> AddPlayerAsync(int id, [FromBody] string playerId, GameService _gameService)
    {
        try
        {
            if (!_gameService.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await _gameService.AddPlayerAsync(id, playerId);

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }

    private static async Task<NoContent> DeleteAsync(int id, GameService _gameService)
    {
        await _gameService.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> UpdateChallengesAsync(int id, GameService _gameService)
    {
        try
        {
            if (!_gameService.Exists(id))
            {
                return TypedResults.NotFound();
            }

            await _gameService.ImportChallengeAsync(id);

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
        catch (HttpRequestException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }
}
