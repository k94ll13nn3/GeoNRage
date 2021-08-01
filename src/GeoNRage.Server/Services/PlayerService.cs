using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos.Players;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class PlayerService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<PlayerStatisticDto>> GetAllStatisticsAsync()
        {
            return await _context
                .Players
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .Select(p => new PlayerStatisticDto
                (
                    p.Id,
                    p.Name,
                    p.PlayerScores.Where(p => (p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 5000),
                    p.PlayerScores.Where(p => (p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 4999),
                    p.PlayerScores.Where(p => (p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)).Count(p => p.PlayerGuesses.Count == 5 && p.PlayerGuesses.All(g => g.Score != null)),
                    p
                        .PlayerScores
                        .Where(ps => ps.PlayerId == p.Id && ps.Challenge.GameId != -1)
                        .Select(ps => new { ps.Challenge.GameId, Sum = ps.PlayerGuesses.Sum(g => g.Score) })
                        .GroupBy(p => p.GameId)
                        .Where(g => g.Count() == 3)
                        .Select(g => new { Id = g.Key, Sum = g.Select(p => p.Sum).Sum() })
                        .OrderByDescending(g => g.Sum)
                        .First()
                        .Sum,
                    p
                        .PlayerScores
                        .Where(ps => ps.PlayerId == p.Id && ps.Challenge.GameId != -1)
                        .Select(ps => new { ps.Challenge.GameId, Sum = ps.PlayerGuesses.Sum(g => g.Score) })
                        .GroupBy(p => p.GameId)
                        .Where(g => g.Count() == 3)
                        .Select(g => new { Id = g.Key, Sum = g.Select(p => p.Sum).Sum() })
                        .OrderByDescending(g => g.Sum)
                        .First()
                        .Id,
                    (int)(p.PlayerScores.Where(p => (p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)).SelectMany(p => p.PlayerGuesses).Select(g => g.Score).Average() ?? 0)
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            return await _context
                .Players
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .Select(p => new PlayerDto
                (
                    p.Id,
                    p.Name
                ))
                .ToListAsync();
        }

        public async Task<PlayerFullDto?> GetFullAsync(string id)
        {
            return await _context
                .Players
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlayerFullDto
                (
                    p.Id,
                    p.Name,
                    p.PlayerScores.Select(ps => new PlayerScoreWithChallengeDto
                    (
                        ps.PlayerGuesses.Sum(g => g.Score),
                        ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null),
                        ps.PlayerGuesses.Select(g => new PlayerGuessDto
                        (
                            g.RoundNumber,
                            g.Score,
                            g.TimedOut,
                            g.TimedOutWithGuess,
                            g.Time,
                            g.Distance
                        )),
                        ps.ChallengeId,
                        ps.Challenge.TimeLimit,
                        ps.Challenge.MapId,
                        ps.Challenge.Map.Name,
                        ps.Challenge.GameId != -1 ? ps.Challenge.GameId : null,
                        ps.Challenge.Game.Date,
                        ps.Challenge.Map.IsMapForGame
                    ))
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<PlayerDto?> GetAsync(string id)
        {
            return await _context
                .Players
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlayerDto
                (
                    p.Id,
                    p.Name
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<PlayerDto?> UpdateAsync(string id, PlayerEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            Player? player = await _context.Players.FindAsync(id);
            if (player is not null)
            {
                player.Name = dto.Name;

                _context.Players.Update(player);
                await _context.SaveChangesAsync();
            }

            return await GetAsync(id);
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
