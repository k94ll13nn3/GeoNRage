using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace GeoNRage.App.Authentication
{
    public class GeoNRageStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthApi _authApi;
        private UserDto _currentUser;

        public GeoNRageStateProvider(IAuthApi authApi)
        {
            _authApi = authApi;
            _currentUser = new UserDto { Claims = new(), IsAuthenticated = false, UserName = string.Empty };
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            UserDto userInfo = await GetCurrentUserAsync();
            if (userInfo.IsAuthenticated)
            {
                IEnumerable<Claim> claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
                identity = new ClaimsIdentity(claims, "Server authentication");
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task LogoutAsync()
        {
            await _authApi.Logout();
            _currentUser = new UserDto { Claims = new(), IsAuthenticated = false, UserName = string.Empty };
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<HttpResponseMessage> LoginAsync(LoginDto loginParameters)
        {
            HttpResponseMessage response = await _authApi.Login(loginParameters);
            if (response.IsSuccessStatusCode)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }

            return response;
        }

        private async Task<UserDto> GetCurrentUserAsync()
        {
            if (_currentUser.IsAuthenticated)
            {
                return _currentUser;
            }

            _currentUser = await _authApi.CurrentUserInfo();
            return _currentUser;
        }
    }
}
