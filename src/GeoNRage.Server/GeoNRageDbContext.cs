using System;
using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server
{
    public class GeoNRageDbContext : IdentityDbContext<User>
    {
        public GeoNRageDbContext(DbContextOptions<GeoNRageDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Value> Values { get; set; } = null!;

        public DbSet<Map> Maps { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<GameMap> GameMaps { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Entity<Game>().HasKey(g => g.Id);
            builder.Entity<Game>().Property(g => g.Name).IsRequired();
            builder.Entity<Game>().Property(g => g.Locked).IsRequired();
            builder.Entity<Game>().Property(g => g.CreationDate).IsRequired();
            builder.Entity<Game>().Property(g => g.Date).IsRequired();
            builder.Entity<Game>().Property(g => g.Rounds).IsRequired();
            builder.Entity<Game>().HasMany(m => m.GameMaps).WithOne(g => g.Game);

            builder.Entity<Player>().HasKey(p => p.Id);
            builder.Entity<Player>().HasMany(p => p.Games).WithMany(g => g.Players);

            builder.Entity<Map>().HasKey(m => m.Id);
            builder.Entity<Map>().HasMany(m => m.GameMaps).WithOne(g => g.Map);

            builder.Entity<Value>().HasKey(v => new { v.GameId, v.MapId, v.PlayerId, v.Round });
            builder.Entity<Value>().HasOne(v => v.Game).WithMany(g => g.Values).HasForeignKey(v => v.GameId);
            builder.Entity<Value>().HasOne(v => v.Player).WithMany(p => p.Values).HasForeignKey(v => v.PlayerId);
            builder.Entity<Value>().HasOne(v => v.Map).WithMany(m => m.Values).HasForeignKey(v => v.MapId);
            builder.Entity<Value>().Property(g => g.Score).IsRequired();

            builder.Entity<GameMap>().HasKey(gm => new { gm.GameId, gm.MapId, gm.Name });

            base.OnModelCreating(builder);
        }
    }
}
