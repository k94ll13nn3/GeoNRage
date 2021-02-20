using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task Login(LoginDto loginRequest);

        [Post("/api/auth/logout")]
        Task Logout();

        [Get("/api/auth/user")]
        Task<UserDto> CurrentUserInfo();
    }
}
