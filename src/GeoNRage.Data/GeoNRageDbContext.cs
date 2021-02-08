using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GeoNRage.Data
{
    public class GeoNRageDbContext : DbContext
    {
        public GeoNRageDbContext(DbContextOptions<GeoNRageDbContext> options) : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Value> Values { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));

            // https://docs.microsoft.com/fr-fr/ef/core/modeling/value-conversions?tabs=data-annotations#collections-of-primitives
            // Keep the cast to ICollection<string>!!
            var stringCollectionValueConverter = new ValueConverter<ICollection<string>, string>(
                v => JsonSerializer.Serialize(v, null),
                v => JsonSerializer.Deserialize<List<string>>(v, null)!);
            var stringCollectionValueComparer = new ValueComparer<ICollection<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode(StringComparison.InvariantCulture))),
                c => (ICollection<string>)c.ToList());

            modelBuilder.Entity<Game>().HasKey(g => g.Id);
            modelBuilder.Entity<Game>().Property(g => g.CreationDate).IsRequired();
            modelBuilder.Entity<Game>().Property(g => g.Rounds).IsRequired();
            modelBuilder.Entity<Game>().Property(g => g.Maps).IsRequired().HasConversion(stringCollectionValueConverter, stringCollectionValueComparer);
            modelBuilder.Entity<Game>().Property(g => g.Players).IsRequired().HasConversion(stringCollectionValueConverter, stringCollectionValueComparer);

            modelBuilder.Entity<Value>().HasKey(v => v.Id);
            modelBuilder.Entity<Value>().HasOne(v => v.Game).WithMany(g => g.Values).HasForeignKey(v => v.GameId);
            modelBuilder.Entity<Value>().Ignore(v => v.Game);
        }
    }
}
