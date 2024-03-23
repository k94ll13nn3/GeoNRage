using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Identity;

public class GeoNRageUserStore(GeoNRageDbContext context, IdentityErrorDescriber describer = null!) : UserStore<
        User,
        IdentityRole,
        GeoNRageDbContext,
        string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityUserToken<string>,
        IdentityRoleClaim<string>
    >(context, describer)
{
    public override Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        return Users.Include(p => p.Player).FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken)!;
    }

    public override Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return Users.Include(p => p.Player).FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)!;
    }
}
