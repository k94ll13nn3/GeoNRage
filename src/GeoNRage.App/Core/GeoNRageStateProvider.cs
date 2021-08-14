using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components.Authorization;

namespace GeoNRage.App.Core;

[AutoConstructor]
public partial class GeoNRageStateProvider : AuthenticationStateProvider
{
    private readonly IAuthApi _authApi;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        UserDto userInfo = await _authApi.CurrentUserInfo();
        if (userInfo.IsAuthenticated)
        {
            IEnumerable<Claim> claims = userInfo.Claims.SelectMany(c => c.Value.Select(v => new Claim(c.Key, v)));
            identity = new ClaimsIdentity(claims, "Server authentication");
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LogoutAsync()
    {
        await _authApi.Logout();
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

    public void NotifyUpdate()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
