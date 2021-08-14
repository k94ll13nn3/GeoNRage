using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Entities;

public class User : IdentityUser
{
    public string? PlayerId { get; set; }

    public Player? Player { get; set; }
}
