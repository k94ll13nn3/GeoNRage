using Refit;

namespace GeoNRage.App.Apis;

public interface IAuthApi
{
    [Post("/api/auth/login")]
    Task<HttpResponseMessage> Login(LoginDto loginRequest);

    [Post("/api/auth/logout")]
    Task Logout();

    [Get("/api/auth/user")]
    Task<UserDto> CurrentUserInfo();

    [Post("/api/auth/edit")]
    Task EditAsync(UserEditDto parameters);
}
