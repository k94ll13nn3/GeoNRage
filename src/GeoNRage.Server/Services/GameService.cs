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
                    .Include(g => g.Challenges)
                    .ThenInclude(c => c.Map)
                    .Include(g => g.Challenges)
                    .ThenInclude(c => c.PlayerScores)
                    .ThenInclude(c => c.Player);
            }

            return await query.ToListAsync();
        }

        public async Task<Game?> GetAsync(int id)
        {
            return await _context
                .Games
                .Include(g => g.Challenges)
                .ThenInclude(c => c.Map)
                .Include(g => g.Challenges)
                .ThenInclude(c => c.PlayerScores)
                .ThenInclude(c => c.Player)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(GameCreateDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            EntityEntry<Game> game = await _context.Games.AddAsync(new Game
            {
                Name = dto.Name,
                Date = dto.Date,
                CreationDate = DateTime.UtcNow,
                Challenges = dto.Challenges.Select(x => new Challenge
                {
                    Link = x.Link,
                    MapId = x.MapId,
                    PlayerScores = dto.PlayerIds.Select(p => new PlayerScore { PlayerId = p }).ToList()
                }).ToList()
            });
            await _context.SaveChangesAsync();

            return game.Entity;
        }

        public async Task<Game?> UpdateAsync(int id, GameEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            Game? game = await GetAsync(id);
            if (game is not null)
            {
                game.Name = dto.Name;
                game.Date = dto.Date;

                _context.Games.Update(game);
                await _context.SaveChangesAsync();
            }

            return game;
        }

        public async Task DeleteAsync(int id)
        {
            Game? game = await _context.Games
                .Include(g => g.Challenges)
                .ThenInclude(c => c.PlayerScores)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (game is not null)
            {
                if (game.Challenges.SelectMany(c => c.PlayerScores).Any(p => p.Round1 > 0 || p.Round2 > 0 || p.Round3 > 0 || p.Round4 > 0 || p.Round5 > 0))
                {
                    throw new InvalidOperationException("Cannot delete an ongoing game.");
                }

                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPlayerAsync(int gameId, string playerId)
        {
            Game? game = await GetAsync(gameId);
            if (game is not null)
            {
                foreach (Challenge challenge in game.Challenges)
                {
                    if (!challenge.PlayerScores.Select(p => p.PlayerId).Contains(playerId) && await _context.Players.AnyAsync(p => p.Id == playerId))
                    {
                        challenge.PlayerScores.Add(new PlayerScore { PlayerId = playerId });
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int newScore)
        {
            Game? game = await _context.Games.FindAsync(gameId);
            if (game is not null)
            {
                PlayerScore playerScore = game.Challenges.First(c => c.Id == challengeId).PlayerScores.First(p => p.PlayerId == playerId);

                switch (round)
                {
                    case 1:
                        playerScore.Round1 = newScore;
                        break;

                    case 2:
                        playerScore.Round2 = newScore;
                        break;

                    case 3:
                        playerScore.Round3 = newScore;
                        break;

                    case 4:
                        playerScore.Round4 = newScore;
                        break;

                    case 5:
                        playerScore.Round5 = newScore;
                        break;

                    default:
                        throw new InvalidOperationException("Invalid round number.");
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
