using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace GeoNRage.Server;

internal sealed class GeoNRageDbContext(DbContextOptions<GeoNRageDbContext> options) : IdentityDbContext<
        User,
        IdentityRole,
        string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>
    >(options), IDataProtectionKeyContext
{
    public DbSet<Game> Games { get; set; } = null!;

    public DbSet<Map> Maps { get; set; } = null!;

    public DbSet<Player> Players { get; set; } = null!;

    public DbSet<Challenge> Challenges { get; set; } = null!;

    public DbSet<PlayerScore> PlayerScores { get; set; } = null!;

    public DbSet<Location> Locations { get; set; } = null!;

    public DbSet<PlayerGuess> PlayerGuesses { get; set; } = null!;

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasCharSet(CharSet.Utf8Mb4, true).UseCollation("utf8mb4_general_ci");

        builder.Entity<Game>().HasKey(g => g.Id);
        builder.Entity<Game>().Property(g => g.Id).UseMySqlIdentityColumn();
        builder.Entity<Game>().HasMany(g => g.Challenges).WithOne(c => c.Game).HasForeignKey(c => c.GameId);

        builder.Entity<Player>().HasKey(p => p.Id);
        builder.Entity<Player>().Property(g => g.IconUrl).HasDefaultValue(Constants.BaseAvatarUrl);
        builder.Entity<Player>().HasOne(p => p.AssociatedPlayer).WithOne().HasForeignKey<Player>(p => p.AssociatedPlayerId);
        builder.Entity<Player>().Property(g => g.Title).HasDefaultValue("Newbie");
        builder.Entity<Player>().Property(p => p.GeoGuessrName).HasDefaultValue("N/A");

        builder.Entity<Map>().HasKey(m => m.Id);
        builder.Entity<Map>().Property(m => m.GeoGuessrName).HasDefaultValue("N/A");

        builder.Entity<Challenge>().HasKey(m => m.Id);
        builder.Entity<Challenge>().HasIndex(g => g.GeoGuessrId).IsUnique();
        builder.Entity<Challenge>().Property(g => g.Id).UseMySqlIdentityColumn();
        builder.Entity<Challenge>().HasOne(c => c.Map).WithMany(m => m.Challenges).HasForeignKey(c => c.MapId);
        builder.Entity<Challenge>().HasMany(c => c.PlayerScores).WithOne(p => p.Challenge).HasForeignKey(p => p.ChallengeId);
        builder.Entity<Challenge>().HasOne(c => c.Creator).WithMany().HasForeignKey(c => c.CreatorId);

        builder.Entity<PlayerScore>().HasKey(p => new { p.ChallengeId, p.PlayerId });
        builder.Entity<PlayerScore>().HasOne(p => p.Player).WithMany(p => p.PlayerScores).HasForeignKey(p => p.PlayerId);
        builder.Entity<PlayerScore>().HasMany(c => c.PlayerGuesses).WithOne(p => p.PlayerScore).HasForeignKey(p => new { p.ChallengeId, p.PlayerId });

        builder.Entity<Location>().HasKey(l => new { l.ChallengeId, l.RoundNumber });
        builder.Entity<Location>().HasOne(l => l.Challenge).WithMany(c => c.Locations).HasForeignKey(l => l.ChallengeId);

        builder.Entity<PlayerGuess>().HasKey(p => new { p.ChallengeId, p.PlayerId, p.RoundNumber });

        builder.Entity<Game>().HasData(new Game { Id = -1, Date = DateTime.MinValue, Name = "Default Game - do not use!" });

        base.OnModelCreating(builder);

        builder.Entity<User>().HasOne(u => u.Player).WithOne().HasForeignKey<User>(u => u.PlayerId);

        builder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId);
        builder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId);

        builder.Entity<DataProtectionKey>().HasKey(d => d.Id);
    }
}
