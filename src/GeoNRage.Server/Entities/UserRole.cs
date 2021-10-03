using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Entities;

public class UserRole : IdentityUserRole<string>
{
    public User User { get; set; } = null!;

    public IdentityRole Role { get; set; } = null!;
}
