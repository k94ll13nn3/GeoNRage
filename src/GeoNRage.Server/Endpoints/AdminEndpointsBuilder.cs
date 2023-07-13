using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GeoNRage.Server.Endpoints;

public static class AdminEndpointsBuilder
{
    public static RouteGroupBuilder MapAdminEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/admin").RequireRole(Roles.Admin).WithTags("Admin");

        group.MapGet("/info", GetAdminInfo);
        group.MapGet("/users", GetAllUsersAsAdminViewAsync).RequireRole(Roles.SuperAdmin);

        return group;
    }

    private static Ok<AdminInfoDto> GetAdminInfo(AdminService _adminService)
    {
        return TypedResults.Ok(_adminService.GetAdminInfo());
    }

    private static async Task<Ok<IEnumerable<UserAminViewDto>>> GetAllUsersAsAdminViewAsync(AdminService _adminService)
    {
        return TypedResults.Ok(await _adminService.GetAllUsersAsAdminViewAsync());
    }
}
