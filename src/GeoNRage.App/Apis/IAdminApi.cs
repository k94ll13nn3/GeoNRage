using Refit;

namespace GeoNRage.App.Apis;

public interface IAdminApi
{
    [Get("/api/admin/info")]
    Task<AdminInfoDto> GetAdminInfoAsync();

    [Get("/api/admin/users")]
    Task<IEnumerable<UserAminViewDto>> GetAllUsersAsAdminViewAsync();
}
