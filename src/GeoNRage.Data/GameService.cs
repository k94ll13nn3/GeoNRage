using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Data
{
    public class GameService
    {
        private readonly GeoNRageDbContext _context;

        public GameService(GeoNRageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GameBase>> GetAllAsync()
        {
            return (await _context.Games.ToListAsync()).Cast<GameBase>();
        }

        public async Task<GameBase> CreateGameAsync(string name, string maps, string columns, string rows)
        {
            var game = await _context.Games.AddAsync(new Game
            {
                Name = name,
                Maps = maps.Split('_'),
                Columns = columns.Split('_'),
                Rows = rows.Split('_'),
            });
            await _context.SaveChangesAsync();

            return game.Entity;
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
