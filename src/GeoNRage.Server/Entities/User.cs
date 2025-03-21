using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Entities;

internal sealed class User : IdentityUser
{
    public string? PlayerId { get; set; }

    public Player? Player { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
