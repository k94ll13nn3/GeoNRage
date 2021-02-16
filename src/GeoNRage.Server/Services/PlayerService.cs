using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Server.Services
{
    public class PlayerService
    {
        private readonly GeoNRageDbContext _context;

        public PlayerService(GeoNRageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.Include(m => m.Games).ToListAsync();
        }

        public async Task<Player> CreateAsync(string name)
        {
            EntityEntry<Player> player = await _context.Players.AddAsync(new Player
            {
                Name = name,
            });

            await _context.SaveChangesAsync();

            return player.Entity;
        }

        public async Task<Player?> UpdateAsync(int id, string name)
        {
            Player? player = await _context.Players.FindAsync(id);
            if (player is not null)
            {
                player.Name = name;

                _context.Players.Update(player);
                await _context.SaveChangesAsync();
            }

            return player;
        }

        public async Task DeleteAsync(int id)
        {
            Player? player = await _context.Players.Include(m => m.Games).FirstAsync(m => m.Id == id);
            if (player is not null)
            {
                if (player.Games.Count > 0)
                {
                    throw new InvalidOperationException("Cannot delete a player in use.");
                }

                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}
