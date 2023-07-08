using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapMapsEndpoints();
        builder.MapLocationsEndpoints();
        builder.MapPlayersEndpoints();
        builder.MapAdminEndpoints();
        builder.MapChallengesEndpoints();
        builder.MapAuthEndpoints();

        return builder;
    }

    public static TBuilder RequireRole<TBuilder>(this TBuilder builder, string role)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder
            .RequireAuthorization(new AuthorizeAttribute() { Roles = role })
            .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized))
            .WithMetadata(new ProducesResponseTypeAttribute(StatusCodes.Status403Forbidden));
    }
}
