using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Data
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
            return await _context.Games.ToListAsync();
        }

        public async Task<IEnumerable<GameBase>> GetAllBaseAsync()
        {
            return (await _context.Games.ToListAsync()).Cast<GameBase>();
        }

        public async Task<GameBase> CreateGameAsync(string name, string maps, string columns, string rows)
        {
            EntityEntry<Game> game = await _context.Games.AddAsync(new Game
            {
                Name = name,
                Maps = maps.Split('_'),
                Columns = columns.Split('_'),
                Rows = rows.Split('_'),
                CreationDate = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return game.Entity;
        }

        public async Task UpdateGameAsync(int id, string name, string maps, string columns, string rows)
        {
            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                game.Name = name;
                game.Maps = maps.Split('_');
                game.Columns = columns.Split('_');
                game.Rows = rows.Split('_');

                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteGameAsync(int id)
        {
            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetGameAsync(int id)
        {
            List<Value> values = await _context.Values.Where(v => v.GameId == id).ToListAsync();
            _context.Values.RemoveRange(values);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.Include(g => g.Values).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task UpdateValueAsync(int gameId, string key, int newScore)
        {
            Value? value = await _context.Values.FirstOrDefaultAsync(x => x.GameId == gameId && x.Key == key);
            if (value is null)
            {
                value = new Value
                {
                    GameId = gameId,
                    Key = key,
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
