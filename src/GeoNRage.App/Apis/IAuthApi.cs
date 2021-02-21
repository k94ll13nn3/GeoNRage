using System.Net.Http;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
using Refit;

namespace GeoNRage.App.Apis
{
    public interface IAuthApi
    {
        [Post("/api/auth/login")]
        Task<HttpResponseMessage> Login(LoginDto loginRequest);

        [Post("/api/auth/logout")]
        Task<HttpResponseMessage> Logout();

        [Get("/api/auth/user")]
        Task<UserDto> CurrentUserInfo();
    }
}
