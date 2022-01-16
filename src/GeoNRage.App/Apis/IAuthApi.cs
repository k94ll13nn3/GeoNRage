using Refit;

namespace GeoNRage.App.Apis;

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<HttpResponseMessage> Login(LoginDto loginRequest);

    [Post("/api/auth/logout")]
    Task Logout();

    [Get("/api/auth/user")]
    [Headers($"{Constants.MapStatusHeaderName}:")]
    Task<UserDto> CurrentUserInfo();

    [Post("/api/auth/edit")]
    Task EditAsync(UserEditDto parameters);

    [Post("/api/auth/edit/admin")]
    Task EditByAdminAsync(UserEditAdminDto parameters);

    [Post("/api/auth/register")]
    Task RegisterAsync(RegisterDto parameters);

    [Delete("/api/auth/{userName}")]
    Task DeleteUserAsync(string userName);
}
