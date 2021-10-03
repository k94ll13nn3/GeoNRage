using System.Security.Claims;

namespace GeoNRage.App.Core;

public static class ClaimsPrincipalExtensions
{
    public static string? PlayerId(this ClaimsPrincipal user)
    {
        return user?.FindFirstValue("PlayerId");
    }
}
