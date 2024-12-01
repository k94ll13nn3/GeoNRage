using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

internal static class LocationsEndpointsBuilder
{
    public static RouteGroupBuilder MapLocationsEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/locations").WithTags("Locations");

        group.MapGet("/", GetAllAsync);

        return group;
    }

    private static async Task<Ok<IEnumerable<LocationDto>>> GetAllAsync(LocationService locationService, HttpContext httpContext)
    {
        return TypedResults.Ok(await locationService.GetAllAsync(httpContext.Request.Headers[Constants.MapStatusHeaderName] == "True"));
    }
}
