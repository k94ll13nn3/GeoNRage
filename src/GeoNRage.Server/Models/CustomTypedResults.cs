using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Models;

internal static class CustomTypedResults
{
    public static ProblemHttpResult Problem(string message)
    {
        return TypedResults.Problem(message, statusCode: StatusCodes.Status400BadRequest, title: "Bad Request");
    }
}
