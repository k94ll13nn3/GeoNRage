using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

public static class MapsEndpointsBuilder
{
    public static RouteGroupBuilder MapMapsEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/maps").WithTags("Maps");

        group.MapGet("/", GetAllAsync);

        group.MapPut("/{id}", UpdateAsync).RequireRole(Roles.Admin);

        group.MapDelete("/{id}", DeleteAsync).RequireRole(Roles.Admin);

        return group;
    }

    private static async Task<Ok<IEnumerable<MapDto>>> GetAllAsync(MapService mapService)
    {
        return TypedResults.Ok(await mapService.GetAllAsync());
    }

    private static async Task<Results<NoContent, NotFound, BadRequest<ApiError>>> UpdateAsync(MapService mapService, string id, MapEditDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        try
        {
            MapDto? updatedMap = await mapService.UpdateAsync(id, dto);
            return updatedMap is not null
                ? TypedResults.NoContent()
                : TypedResults.NotFound();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }

    private static async Task<Results<NoContent, BadRequest<ApiError>>> DeleteAsync(MapService mapService, string id)
    {
        try
        {
            await mapService.DeleteAsync(id);
            return TypedResults.NoContent();
        }
        catch (InvalidOperationException e)
        {
            return TypedResults.BadRequest(new ApiError(e.Message));
        }
    }
}
