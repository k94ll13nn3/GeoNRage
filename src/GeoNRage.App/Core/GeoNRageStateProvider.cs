using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components.Authorization;

namespace GeoNRage.App.Core;

public class GeoNRageStateProvider : AuthenticationStateProvider
{
    private readonly IAuthApi _authApi;
    private UserDto _currentUser;

    public GeoNRageStateProvider(IAuthApi authApi)
    {
        _authApi = authApi;
        _currentUser = new UserDto(false, string.Empty, new());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        UserDto userInfo = await GetCurrentUserAsync();
        if (userInfo.IsAuthenticated)
        {
            IEnumerable<Claim> claims = _currentUser.Claims.SelectMany(c => c.Value.Select(v => new Claim(c.Key, v)));
            identity = new ClaimsIdentity(claims, "Server authentication");
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LogoutAsync()
    {
        await _authApi.Logout();
        _currentUser = new UserDto(false, string.Empty, new());
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
