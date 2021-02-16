using System;
using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server
{
    public class GeoNRageDbContext : DbContext
    {
        public GeoNRageDbContext(DbContextOptions<GeoNRageDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Value> Values { get; set; } = null!;

        public DbSet<Map> Maps { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.Entity<Game>().HasKey(g => g.Id);
            modelBuilder.Entity<Game>().Property(g => g.Name).IsRequired();
            modelBuilder.Entity<Game>().Property(g => g.Locked).IsRequired();
            modelBuilder.Entity<Game>().Property(g => g.CreationDate).IsRequired();
            modelBuilder.Entity<Game>().Property(g => g.Rounds).IsRequired();

            modelBuilder.Entity<Player>().HasKey(p => p.Id);
            modelBuilder.Entity<Player>().HasMany(p => p.Games).WithMany(g => g.Players);

            modelBuilder.Entity<Map>().HasKey(m => m.Id);
            modelBuilder.Entity<Map>().HasMany(m => m.Games).WithMany(g => g.Maps);

            modelBuilder.Entity<Value>().HasKey(v => new { v.GameId, v.MapId, v.PlayerId, v.Round });
            modelBuilder.Entity<Value>().HasOne(v => v.Game).WithMany(g => g.Values).HasForeignKey(v => v.GameId);
            modelBuilder.Entity<Value>().HasOne(v => v.Player).WithMany(p => p.Values).HasForeignKey(v => v.PlayerId);
            modelBuilder.Entity<Value>().HasOne(v => v.Map).WithMany(m => m.Values).HasForeignKey(v => v.MapId);
            modelBuilder.Entity<Value>().Property(g => g.Score).IsRequired();
        }
    }
}
