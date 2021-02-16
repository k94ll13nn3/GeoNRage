using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Server.Services
{
    public class GameService
    {
        private readonly GeoNRageDbContext _context;

        public GameService(GeoNRageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games.OrderByDescending(g => g.CreationDate).ToListAsync();
        }

        public async Task<Game?> GetAsync(int id)
        {
            return await _context
                .Games
                .Include(g => g.Values)
                .Include(g => g.Maps)
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(string name, ICollection<Map> maps, ICollection<Player> players)
        {
            _ = maps ?? throw new ArgumentNullException(nameof(maps));
            _ = players ?? throw new ArgumentNullException(nameof(players));

            EntityEntry<Game> game = await _context.Games.AddAsync(new Game
            {
                Name = name,
                Maps = maps,
                Players = players,
                Rounds = 5,
                CreationDate = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return game.Entity;
        }

        public async Task UpdateAsync(int id, string name, ICollection<Map> maps, ICollection<Player> players)
        {
            _ = maps ?? throw new ArgumentNullException(nameof(maps));
            _ = players ?? throw new ArgumentNullException(nameof(players));

            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot update a locked game.");
                }

                game.Name = name;
                game.Maps = maps;
                game.Players = players;

                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LockAsync(int id)
        {
            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                game.Locked = true;
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetAsync(int id)
        {
            List<Value> values = await _context.Values.Where(v => v.GameId == id).ToListAsync();
            _context.Values.RemoveRange(values);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateValueAsync(int gameId, int mapId, int playerId, int round, int newScore)
        {
            Game? game = await _context.Games.FindAsync(gameId);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot update a locked game.");
                }

                Value? value = await _context.Values.FirstOrDefaultAsync(x => x.GameId == gameId && x.MapId == mapId && x.PlayerId == playerId && x.Round == round);
                if (value is null)
                {
                    value = new Value
                    {
                        GameId = gameId,
                        MapId = mapId,
                        PlayerId = playerId,
                        Round = round,
                        Score = newScore,
                    };
                    _context.Values.Add(value);
                }
                else
                {
                    value.Score = newScore;
                    _context.Values.Update(value);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
