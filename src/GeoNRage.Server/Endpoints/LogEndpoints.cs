using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

internal static class LogsEndpointsBuilder
{
    public static RouteGroupBuilder MapLogsEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/logs").WithTags("Logs");

        group.MapPost("/", PostLog);

        return group;
    }

    private static NoContent PostLog(ErrorLog error, ILogger<ErrorLog> logger)
    {
        using (Loggers.Scope(logger, error.Source, error.StackTrace))
        {
            Loggers.LogErrorFromApp(logger, error.Message, null);
        }

        return TypedResults.NoContent();
    }
}
