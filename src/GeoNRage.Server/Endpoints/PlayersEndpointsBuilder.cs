using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

public static class PlayersEndpointsBuilder
{
    public static RouteGroupBuilder MapPlayersEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/players").WithTags("Players");

        group.MapGet("/", GetAllAsync);
        group.MapGet("/admin-view", GetAllAsAdminViewAsync).RequireRole(Roles.Admin);
        group.MapGet("/statistics", GetAllStatisticsAsync);
        group.MapGet("/{id}/full", GetFullAsync);

        group.MapPut("/{id}", UpdateAsync).RequireRole(Roles.Admin);

        group.MapPost("/{id}/update-geoguessr-profile", UpdateGeoGuessrProfileAsync).RequireRole(Roles.Admin);

        group.MapDelete("/{id}", DeleteAsync).RequireRole(Roles.Admin);

        return group;
    }

    private static async Task<Ok<IEnumerable<PlayerDto>>> GetAllAsync(PlayerService playerService)
    {
        return TypedResults.Ok(await playerService.GetAllAsync());
    }

    private static async Task<Ok<IEnumerable<PlayerAdminViewDto>>> GetAllAsAdminViewAsync(PlayerService playerService)
    {
        return TypedResults.Ok(await playerService.GetAllAsAdminViewAsync());
    }

    private static async Task<Ok<IEnumerable<PlayerStatisticDto>>> GetAllStatisticsAsync(PlayerService playerService, HttpContext httpContext)
    {
        return TypedResults.Ok(await playerService.GetAllStatisticsAsync(httpContext.Request.Headers[Constants.MapStatusHeaderName] == "True"));
    }

    private static async Task<Results<Ok<PlayerFullDto>, NotFound>> GetFullAsync(string id, PlayerService playerService, HttpContext httpContext)
    {
        PlayerFullDto? player = await playerService.GetFullAsync(id, httpContext.Request.Headers[Constants.MapStatusHeaderName] == "True");
        if (player is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(player);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> UpdateAsync(string id, PlayerEditDto dto, PlayerService playerService)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            PlayerDto? updatedPlayer = await playerService.UpdateAsync(id, dto);
            if (updatedPlayer is null)
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

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> UpdateGeoGuessrProfileAsync(string id, PlayerService playerService)
    {
        try
        {
            PlayerDto? updatedPlayer = await playerService.UpdateGeoGuessrProfileAsync(id);
            if (updatedPlayer is null)
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

    private static async Task<Results<NoContent, BadRequest<ApiError>>> DeleteAsync(string id, PlayerService playerService)
    {
        try
        {
            await playerService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }
}
