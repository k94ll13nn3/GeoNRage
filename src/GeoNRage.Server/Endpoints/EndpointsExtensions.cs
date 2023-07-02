using Microsoft.AspNetCore.Authorization;

namespace GeoNRage.Server.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapMapsEndpoints();
        builder.MapLocationsEndpoints();
        builder.MapPlayersEndpoints();

        return builder;
    }

    public static TBuilder RequireRole<TBuilder>(this TBuilder builder, string role)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.RequireAuthorization(new AuthorizeAttribute() { Roles = role });
    }
}
