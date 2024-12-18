using System.Security.Claims;
using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Identity;

internal sealed class GeoNRageUserClaimsPrincipalFactory(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<User, IdentityRole>(userManager, roleManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        ClaimsIdentity? identity = await base.GenerateClaimsAsync(user);
        if (user is not null)
        {
            identity.AddClaim(new Claim("ProfilePicture", (user.Player?.IconUrl ?? Constants.BaseAvatarUrl).ToString()));
            if (user.PlayerId is not null)
            {
                identity.AddClaim(new Claim("PlayerId", user.PlayerId));
            }
        }

        return identity;
    }
}
