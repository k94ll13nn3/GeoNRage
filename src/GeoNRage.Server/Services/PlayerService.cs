using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<PlayerStatisticDto>> GetAllStatisticsAsync(bool takeAllMaps)
        {
            return await _context
                .Players
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .Select(p => new PlayerStatisticDto
                (
                    p.Id,
                    p.Name,
                    p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 5000),
                    p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 4999),
                    p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).Count(p => p.PlayerGuesses.Count == 5 && p.PlayerGuesses.All(g => g.Score != null)),
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
                    (int)(p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Select(g => g.Score).Average() ?? 0)
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

        public async Task<PlayerFullDto?> GetFullAsync(string id, bool takeAllMaps)
        {
            IEnumerable<PlayerMapDto> mapsSummary = _context
                .Maps
                .WhereIf(!takeAllMaps, m => m.IsMapForGame || m.Challenges.Any(c => c.GameId != -1))
                .Select(m => new PlayerMapDto
                 (
                     m.Name,
                     m.Challenges.Where(c => takeAllMaps || ((c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || c.Map.IsMapForGame))).SelectMany(c => c.PlayerScores.Where(ps => ps.PlayerId == id)).Select(ps => ps.PlayerGuesses.Sum(g => g.Score)).OrderByDescending(s => s).First(),
                     (int?)m.Challenges.Where(c => takeAllMaps || ((c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || c.Map.IsMapForGame))).SelectMany(c => c.PlayerScores.Where(ps => ps.PlayerId == id)).Select(ps => ps.PlayerGuesses.Sum(g => g.Score)).Average()
                 )).AsEnumerable();

            IEnumerable<PlayerChallengeDto> challengesNotDone = _context
                .Challenges
                .WhereIf(!takeAllMaps, c => (c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || c.Map.IsMapForGame))
                .Where(c => !c.PlayerScores.Any(ps => ps.PlayerId == id && ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null) && ps.ChallengeId == c.Id))
                .Select(c => new PlayerChallengeDto
                (
                    c.Id,
                    c.GameId == -1 ? null : c.GameId, c.Map.Name, null
                ))
                .AsEnumerable();

            PlayerFullDto? player = await _context
                .Players
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PlayerFullDto
                (
                    p.Id,
                    p.Name,
                    p
                        .PlayerScores
                        .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                        .Where(ps => ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null))
                        .Select(ps => new PlayerChallengeDto
                        (
                            ps.ChallengeId,
                            ps.Challenge.GameId == -1 ? null : ps.Challenge.GameId,
                            ps.Challenge.Map.Name,
                            ps.PlayerGuesses.Sum(g => g.Score)
                        )),
                    null!,
                    new PlayerFullStatisticDto
                    (
                        p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 5000),
                        p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 4999),
                        p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).Count(p => p.PlayerGuesses.Count == 5 && p.PlayerGuesses.All(g => g.Score != null)),
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
                        (int)(p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Select(g => g.Score).Average() ?? 0),
                        p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).Select(p => p.PlayerGuesses.Sum(g => g.Score)).Count(s => s == 25000),
                        (int)(p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).Select(p => p.PlayerGuesses.Sum(g => g.Score)).Average() ?? 0),
                        p.PlayerScores.Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame))).SelectMany(p => p.PlayerGuesses).Count(g => g.Score == 0)
                    ),
                    null!,
                    p
                        .PlayerScores
                        .Where(ps => ps.PlayerId == p.Id && ps.Challenge.GameId != -1)
                        .Select(ps => new { ps.Challenge.GameId, ps.Challenge.Game.Date, Sum = ps.PlayerGuesses.Sum(g => g.Score) })
                        .GroupBy(p => new { p.GameId, p.Date })
                        .Where(g => g.Count() == 3)
                        .OrderBy(g => g.Key.Date)
                        .Select(g => new PlayerGameDto
                        (
                            g.Key.GameId,
                            g.Select(p => p.Sum).Sum() ?? 0,
                            g.Key.Date
                        ))
                ))
                .FirstOrDefaultAsync();

            if (player is not null)
            {
                player = player with { MapsSummary = mapsSummary, ChallengesNotDone = challengesNotDone };
            }

            return player;
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
