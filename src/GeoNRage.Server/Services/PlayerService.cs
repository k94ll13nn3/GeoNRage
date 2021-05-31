using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class PlayerService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<Player>> GetAllAsync(bool includeNavigation)
        {
            IQueryable<Player> query = _context.Players;
            if (includeNavigation)
            {
                query = query
                    .Include(p => p.PlayerScores)
                    .ThenInclude(p => p.Challenge)
                    .ThenInclude(c => c.Game)
                    .Include(p => p.PlayerScores)
                    .ThenInclude(p => p.Challenge)
                    .ThenInclude(c => c.Map);
            }

            List<Player> players = await query.ToListAsync();

            return players;
        }

        public async Task<Player?> GetFullAsync(string id)
        {
            return await _context.Players
                .Include(p => p.PlayerScores)
                .ThenInclude(p => p.Challenge)
                .ThenInclude(c => c.Game)
                .Include(p => p.PlayerScores)
                .ThenInclude(p => p.Challenge)
                .ThenInclude(c => c.Map)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Player> CreateAsync(PlayerCreateDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            if (await _context.Players.AnyAsync(p => p.Id == dto.Id))
            {
                throw new InvalidOperationException($"Player '{dto.Id}' already exists.");
            }

            EntityEntry<Player> player = await _context.Players.AddAsync(new Player
            {
                Id = dto.Id,
                Name = dto.Name,
            });

            await _context.SaveChangesAsync();

            return player.Entity;
        }

        public async Task<Player?> UpdateAsync(string id, PlayerEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            Player? player = await _context.Players.FindAsync(id);
            if (player is not null)
            {
                player.Name = dto.Name;

                _context.Players.Update(player);
                await _context.SaveChangesAsync();
            }

            return player;
        }

        public async Task DeleteAsync(string id)
        {
            Player? player = await _context.Players.FindAsync(id);
            if (player is not null)
            {
                if (await _context.PlayerScores.AnyAsync(ps => ps.PlayerId == id))
                {
                    throw new InvalidOperationException("Cannot delete a player in use.");
                }

                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }
    }
}
