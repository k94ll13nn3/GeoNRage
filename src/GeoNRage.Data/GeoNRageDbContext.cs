using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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

            var stringCollectionConverter = new ValueConverter<ICollection<string>, string>(v => string.Join("_", v), v => v.Split(new[] { '_' }));

            modelBuilder.Entity<Game>().HasKey(g => g.Id);
            modelBuilder.Entity<Game>().Property(g => g.Columns).HasConversion(stringCollectionConverter);
            modelBuilder.Entity<Game>().Property(g => g.Rows).HasConversion(stringCollectionConverter);
            modelBuilder.Entity<Game>().Property(g => g.Maps).HasConversion(stringCollectionConverter);

            modelBuilder.Entity<Value>().HasKey(v => v.Id);
            modelBuilder.Entity<Value>().HasOne(v => v.Game).WithMany(g => g.Values).HasForeignKey(v => v.GameId);
            modelBuilder.Entity<Value>().Ignore(v => v.Game);
        }
    }
}
