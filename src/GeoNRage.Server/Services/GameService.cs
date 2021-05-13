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
    public partial class GameService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<Game>> GetAllAsync(bool includeNavigation)
        {
            IQueryable<Game> query = _context.Games.OrderByDescending(g => g.Date);
            if (includeNavigation)
            {
                query = query
                    .Include(g => g.Values)
                    .Include(g => g.GameMaps).ThenInclude(gm => gm.Map)
                    .Include(g => g.Players);
            }

            return await query.ToListAsync();
        }

        public async Task<Game?> GetAsync(int id)
        {
            return await _context
                .Games
                .Include(g => g.Values)
                .Include(g => g.GameMaps).ThenInclude(gm => gm.Map)
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(string name, DateTime date, ICollection<GameMapCreateOrEditDto> maps, ICollection<int> playerIds)
        {
            _ = maps ?? throw new ArgumentNullException(nameof(maps));
            _ = playerIds ?? throw new ArgumentNullException(nameof(playerIds));

            List<Player> players = await _context.Players.Where(p => playerIds.Contains(p.Id)).ToListAsync();

            EntityEntry<Game> game = await _context.Games.AddAsync(new Game
            {
                Name = name,
                Date = date,
                GameMaps = maps.Select(x => new GameMap { MapId = x.MapId, Link = x.Link, Name = x.Name }).ToList(),
                Players = players,
                Rounds = 5,
                CreationDate = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();

            return game.Entity;
        }

        public async Task<Game?> UpdateAsync(int id, string name, DateTime date, ICollection<GameMapCreateOrEditDto> maps, ICollection<int> playerIds)
        {
            _ = maps ?? throw new ArgumentNullException(nameof(maps));
            _ = playerIds ?? throw new ArgumentNullException(nameof(playerIds));

            Game? game = await GetAsync(id);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot update a locked game.");
                }

                if (game.Values.Any(v => v.Score > 0))
                {
                    if (game.Players.Select(p => p.Id).Any(id => !playerIds.Contains(id)))
                    {
                        throw new InvalidOperationException("Cannot remove a player from an ongoing game.");
                    }

                    if (game.GameMaps.Select(m => m.MapId).Any(id => !maps.Select(x => x.MapId).Contains(id)))
                    {
                        throw new InvalidOperationException("Cannot remove a map from an ongoing game.");
                    }
                }

                List<Player> players = await _context.Players.Where(p => playerIds.Contains(p.Id)).ToListAsync();

                game.Name = name;
                game.Date = date;
                game.GameMaps = maps.Select(x => new GameMap { MapId = x.MapId, Link = x.Link, Name = x.Name, GameId = game.Id }).ToList();
                game.Players = players;

                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            return game;
        }

        public async Task DeleteAsync(int id)
        {
            Game? game = await _context.Games.Include(g => g.Values).FirstOrDefaultAsync(g => g.Id == id);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot delete a locked game.");
                }

                if (game.Values.Any(v => v.Score > 0))
                {
                    throw new InvalidOperationException("Cannot delete an ongoing game.");
                }

                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPlayerAsync(int gameId, int playerId)
        {
            Game? game = await GetAsync(gameId);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot update a locked game.");
                }

                if (!game.Players.Select(p => p.Id).Contains(playerId))
                {
                    Player? player = await _context.Players.FindAsync(playerId);
                    if (player is not null)
                    {
                        game.Players.Add(player);
                        await _context.SaveChangesAsync();
                    }
                }
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

        public async Task UnlockAsync(int id)
        {
            Game? game = await _context.Games.FindAsync(id);
            if (game is not null)
            {
                game.Locked = false;
                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetAsync(int id)
        {
            Game? game = await _context.Games.Include(g => g.Values).FirstOrDefaultAsync(g => g.Id == id);
            if (game is not null)
            {
                if (game.Locked)
                {
                    throw new InvalidOperationException("Cannot reset a locked game.");
                }

                List<Value> values = await _context.Values.Where(v => v.GameId == id).ToListAsync();
                _context.Values.RemoveRange(values);
                await _context.SaveChangesAsync();
            }
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
