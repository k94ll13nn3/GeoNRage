using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

public static class AdminEndpointsBuilder
{
    public static RouteGroupBuilder MapAdminEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/admin").RequireRole(Roles.Admin).WithTags("Admin");

        group.MapGet("/info", GetAdminInfoAsync);
        group.MapGet("/users", GetAllUsersAsAdminViewAsync).RequireRole(Roles.SuperAdmin);

        group.MapPost("/clear-logs", ClearLogsAsync);

        return group;
    }

    private static async Task<Ok<AdminInfoDto>> GetAdminInfoAsync(AdminService _adminService)
    {
        return TypedResults.Ok(await _adminService.GetAdminInfoAsync());
    }

    private static async Task<NoContent> ClearLogsAsync(AdminService _adminService)
    {
        await _adminService.ClearLogsAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Ok<IEnumerable<UserAminViewDto>>> GetAllUsersAsAdminViewAsync(AdminService _adminService)
    {
        return TypedResults.Ok(await _adminService.GetAllUsersAsAdminViewAsync());
    }
}
