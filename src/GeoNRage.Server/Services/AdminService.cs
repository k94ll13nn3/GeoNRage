using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services;

[AutoConstructor]
public partial class AdminService
{
    private readonly GeoNRageDbContext _context;

    public AdminInfoDto GetAdminInfo()
    {
        var tables = new List<TableInfoDto>
            {
                new (nameof(GeoNRageDbContext.Games), _context.Games.Count()),
                new (nameof(GeoNRageDbContext.Maps), _context.Maps.Count()),
                new (nameof(GeoNRageDbContext.Players), _context.Players.Count()),
                new (nameof(GeoNRageDbContext.Challenges), _context.Challenges.Count()),
                new (nameof(GeoNRageDbContext.PlayerScores), _context.PlayerScores.Count()),
                new (nameof(GeoNRageDbContext.Locations), _context.Locations.Count()),
                new (nameof(GeoNRageDbContext.PlayerGuesses), _context.PlayerGuesses.Count()),
                new (nameof(GeoNRageDbContext.RoleClaims), _context.RoleClaims.Count()),
                new (nameof(GeoNRageDbContext.Roles), _context.Roles.Count()),
                new (nameof(GeoNRageDbContext.UserClaims), _context.UserClaims.Count()),
                new (nameof(GeoNRageDbContext.UserLogins), _context.UserLogins.Count()),
                new (nameof(GeoNRageDbContext.UserRoles), _context.UserRoles.Count()),
                new (nameof(GeoNRageDbContext.Users), _context.Users.Count()),
                new (nameof(GeoNRageDbContext.UserTokens), _context.UserTokens.Count()),
            };

        return new AdminInfoDto(tables);
    }

    public async Task<IEnumerable<UserAminViewDto>> GetAllUsersAsAdminViewAsync()
    {
        return await _context.Users
            .Select(u => new UserAminViewDto(
                u.UserName ?? string.Empty,
                u.PlayerId,
                u.Player != null ? u.Player.Name : null,
                u.UserRoles.Select(ur => ur.Role.Name ?? string.Empty)
            ))
            .ToListAsync();
    }
}
