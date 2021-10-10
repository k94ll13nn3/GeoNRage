using Refit;

namespace GeoNRage.App.Apis;

public interface IAdminApi
{
    [Get("/api/admin/info")]
    Task<AdminInfoDto> GetAdminInfoAsync();

    [Post("/api/admin/clear-logs")]
    Task ClearLogsAsync();

    [Get("/api/admin/users")]
    Task<IEnumerable<UserAminViewDto>> GetAllUsersAsAdminViewAsync();
}
