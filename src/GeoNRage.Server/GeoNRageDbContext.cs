using System;
using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server
{
    public class GeoNRageDbContext : IdentityDbContext<IdentityUser>
    {
        public GeoNRageDbContext(DbContextOptions<GeoNRageDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Map> Maps { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Challenge> Challenges { get; set; } = null!;

        public DbSet<PlayerScore> PlayerScores { get; set; } = null!;

        public DbSet<Location> Locations { get; set; } = null!;

        public DbSet<PlayerGuess> PlayerGuesses { get; set; } = null!;

        public DbSet<Log> Logs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Entity<Game>().HasKey(g => g.Id);
            builder.Entity<Game>().Property(g => g.Id).UseMySqlIdentityColumn();
            builder.Entity<Game>().Property(g => g.Name).IsRequired();
            builder.Entity<Game>().Property(g => g.CreationDate).IsRequired();
            builder.Entity<Game>().Property(g => g.Date).IsRequired();
            builder.Entity<Game>().HasMany(g => g.Challenges).WithOne(c => c.Game).HasForeignKey(c => c.GameId);

            builder.Entity<Player>().HasKey(p => p.Id);
            builder.Entity<Player>().Property(g => g.Name).IsRequired();

            builder.Entity<Map>().HasKey(m => m.Id);
            builder.Entity<Map>().Property(g => g.Name).IsRequired();

            builder.Entity<Challenge>().HasKey(m => m.Id);
            builder.Entity<Challenge>().HasIndex(g => g.GeoGuessrId).IsUnique();
            builder.Entity<Challenge>().Property(g => g.GeoGuessrId).IsRequired();
            builder.Entity<Challenge>().Property(g => g.UpdatedAt).HasDefaultValue(DateTime.MinValue);
            builder.Entity<Challenge>().Property(g => g.Id).UseMySqlIdentityColumn();
            builder.Entity<Challenge>().HasOne(c => c.Map).WithMany(m => m.Challenges).HasForeignKey(c => c.MapId).IsRequired();
            builder.Entity<Challenge>().HasMany(c => c.PlayerScores).WithOne(p => p.Challenge).HasForeignKey(p => p.ChallengeId);
            builder.Entity<Challenge>().HasOne(c => c.Creator).WithMany().HasForeignKey(c => c.CreatorId);

            builder.Entity<PlayerScore>().HasKey(p => new { p.ChallengeId, p.PlayerId });
            builder.Entity<PlayerScore>().HasOne(p => p.Player).WithMany(p => p.PlayerScores).HasForeignKey(p => p.PlayerId).IsRequired();
            builder.Entity<PlayerScore>().HasMany(c => c.PlayerGuesses).WithOne(p => p.PlayerScore).HasForeignKey(p => new { p.ChallengeId, p.PlayerId });

            builder.Entity<Location>().HasKey(l => new { l.ChallengeId, l.RoundNumber });
            builder.Entity<Location>().HasOne(l => l.Challenge).WithMany(c => c.Locations).HasForeignKey(l => l.ChallengeId).IsRequired();
            builder.Entity<Location>().Property(l => l.RoundNumber).IsRequired();
            builder.Entity<Location>().Property(l => l.Latitude).IsRequired();
            builder.Entity<Location>().Property(l => l.Longitude).IsRequired();

            builder.Entity<PlayerGuess>().HasKey(p => new { p.ChallengeId, p.PlayerId, p.RoundNumber });

            builder.Entity<Log>().HasKey(p => p.Id);

            builder.Entity<Game>().HasData(new Game { Id = -1, CreationDate = DateTime.MinValue, Date = DateTime.MinValue, Name = "Default Game - do not use!" });

            base.OnModelCreating(builder);
        }
    }
}
