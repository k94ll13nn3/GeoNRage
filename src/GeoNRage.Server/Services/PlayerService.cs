using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GeoNRage.Server.Services;

[AutoConstructor]
internal sealed partial class PlayerService
{
    private readonly GeoNRageDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly GeoGuessrService _geoGuessrService;

    public async Task<IEnumerable<PlayerStatisticDto>> GetAllStatisticsAsync(bool takeAllMaps)
    {
        return await _context
            .Players
            .OrderBy(p => p.Name)
            .Select(p => new PlayerStatisticDto
            (
                p.Id,
                p.Name,
                p.IconUrl,
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .SelectMany(p => p.PlayerGuesses)
                    .Count(g => g.Score == 5000),
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .SelectMany(p => p.PlayerGuesses)
                    .Count(g => g.Score == 4999),
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .Count(p => p.PlayerGuesses.Count == 5 && p.PlayerGuesses.All(g => g.Score != null)),
                p
                    .PlayerScores
                    .Where(ps => ps.PlayerId == p.Id && ps.Challenge.GameId != -1 && (takeAllMaps || (ps.Challenge.TimeLimit ?? 300) == 300))
                    .Select(ps => new { ps.Challenge.GameId, ps.Sum })
                    .GroupBy(p => p.GameId)
                    .Where(g => g.Count() == 3)
                    .Select(g => new { Id = g.Key, Sum = g.Sum(p => p.Sum) })
                    .OrderByDescending(g => g.Sum)
                    .First()
                    .Sum,
                p
                    .PlayerScores
                    .Where(ps => ps.PlayerId == p.Id && ps.Challenge.GameId != -1 && (takeAllMaps || (ps.Challenge.TimeLimit ?? 300) == 300))
                    .Select(ps => new { ps.Challenge.GameId, ps.Sum })
                    .GroupBy(p => p.GameId)
                    .Where(g => g.Count() == 3)
                    .Select(g => new { Id = g.Key, Sum = g.Sum(p => p.Sum) })
                    .OrderByDescending(g => g.Sum)
                    .First()
                    .Id,
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .SelectMany(p => p.PlayerGuesses)
                    .Average(g => g.Score)
            ))
            .ToListAsync();
    }

    public Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        return _cache.GetOrCreateAsync(CacheKeys.PlayerServiceGetAllAsync, GetAllFactory)!;

        async Task<IEnumerable<PlayerDto>> GetAllFactory(ICacheEntry entry)
        {
            List<PlayerDto> players = await _context
                .Players
                .OrderBy(p => p.Name)
                .Select(p => new PlayerDto
                (
                    p.Id,
                    p.Name
                ))
                .ToListAsync();

            entry.SetSlidingExpiration(TimeSpan.FromDays(1)).SetSize(players.Count);

            return players;
        }
    }

    public async Task<IEnumerable<PlayerAdminViewDto>> GetAllAsAdminViewAsync()
    {
        return await _context
            .Players
            .OrderBy(p => p.Name)
            .Select(p => new PlayerAdminViewDto
            (
                p.Id,
                p.Name,
                p.AssociatedPlayerId,
                p.AssociatedPlayer == null ? null : p.AssociatedPlayer.Name,
                p.Title,
                p.LastGeoGuessrUpdate,
                p.GeoGuessrName
            ))
            .ToListAsync();
    }

    public async Task<PlayerFullDto?> GetFullAsync(string id, bool takeAllMaps)
    {
        List<PlayerMapDto> mapsSummary = await _context
            .Maps
            .WhereIf(!takeAllMaps, m => m.IsMapForGame || m.Challenges.Any(c => c.GameId != -1))
            .OrderBy(m => m.Name)
            .Select(m => new
            {
                m.Name,
                Challenges = m.Challenges
                    .Where(c => takeAllMaps || ((c.TimeLimit ?? 300) == 300))
                    .SelectMany(c => c.PlayerScores.Where(ps => ps.PlayerId == id))
            })
            .Select(m => new PlayerMapDto
             (
                 m.Name,
                 m.Challenges.Select(ps => ps.Sum).OrderByDescending(s => s).First(),
                 m.Challenges.SelectMany(ps => ps.PlayerGuesses).Average(g => g.Score),
                 m.Challenges.SelectMany(ps => ps.PlayerGuesses).Average(g => g.Distance),
                 m.Challenges.SelectMany(ps => ps.PlayerGuesses).Average(g => g.Time)
             ))
            .ToListAsync();

        List<PlayerChallengeDto> challengesNotDone = await _context
            .Challenges
            .WhereIf(!takeAllMaps, c => (c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || c.Map.IsMapForGame))
            .Where(c => !c.PlayerScores.Any(ps => ps.PlayerId == id && ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null) && ps.ChallengeId == c.Id))
            .Select(c => new PlayerChallengeDto
            (
                c.Id,
                c.GameId == -1 ? null : c.GameId,
                c.Map.Name,
                null,
                null
            ))
            .ToListAsync();

        List<PlayerGuess> playerGuesses = await _context
            .PlayerGuesses
            .Where(g => g.PlayerId == id)
            .WhereIf(!takeAllMaps, g => (g.PlayerScore.Challenge.TimeLimit ?? 300) == 300 && (g.PlayerScore.Challenge.GameId != -1 || g.PlayerScore.Challenge.Map.IsMapForGame))
            .AsNoTracking()
            .ToListAsync();

        List<PlayerGameDto> gameHistory = [.. (await GetGamesWithPlayersScoreAsync(takeAllMaps)).GetValueOrDefault(id, []).OrderBy(g => g.GameDate)];

        List<PlayerChallengeDto> challengesDones = await _context.PlayerScores
            .Where(p => p.PlayerId == id)
            .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
            .Where(ps => ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null))
            .OrderBy(c => c.ChallengeId)
            .Select(ps => new PlayerChallengeDto
            (
                ps.ChallengeId,
                ps.Challenge.GameId == -1 ? null : ps.Challenge.GameId,
                ps.Challenge.Map.Name,
                ps.Sum,
                ps.PlayerGuesses.Sum(g => g.Time)
            ))
            .ToListAsync();

        Player? player = await _context
            .Players
            .AsNoTracking()
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (player is not null)
        {
            PlayerGameDto? bestGame = gameHistory.OrderByDescending(g => g.Sum).FirstOrDefault();
            return new PlayerFullDto
            (
                Id: player.Id,
                Name: player.Name,
                Title: player.Title,
                IconUrl: player.IconUrl,
                ChallengesDone: challengesDones,
                ChallengesNotDone: challengesNotDone,
                Statistics: new PlayerFullStatisticDto
                (
                    NumberOf5000: playerGuesses.Count(g => g.Score == 5000),
                    NumberOf4999: playerGuesses.Count(g => g.Score == 4999),
                    ChallengesCompleted: challengesDones.Count,
                    BestGameSum: bestGame?.Sum,
                    BestGameId: bestGame?.GameId,
                    RoundAverage: playerGuesses.Average(g => g.Score),
                    NumberOf25000: challengesDones.Count(c => c.Sum == 25000),
                    MapAverage: challengesDones.Average(c => c.Sum),
                    NumberOf0: playerGuesses.Count(g => g.Score == 0),
                    TimeByRoundAverage: playerGuesses.Average(g => g.Time),
                    DistanceAverage: playerGuesses.Average(g => g.Distance),
                    StepsAverage: playerGuesses.Average(g => g.Steps),
                    NumberOf0Steps: playerGuesses.Count(g => g.Steps == 0),
                    NumberOfTimeOut: playerGuesses.Count(g => g.TimedOut),
                    NumberOfTimeOutWithGuess: playerGuesses.Count(g => g.TimedOutWithGuess),
                    TotalTime: playerGuesses.Count > 0 ? playerGuesses.Sum(g => g.Time) : null,
                    TotalDistance: playerGuesses.Count > 0 ? playerGuesses.Sum(g => g.Distance) : null,
                    Best5000Time: playerGuesses.Where(g => g.Score == 5000 && g.Time is not null).Min(g => g.Time),
                    Best25000Time: challengesDones.Where(c => c.Sum == 25000).Min(s => s.Time),
                    AverageOf5000ByGame: gameHistory.Count == 0 ? 0 : gameHistory.Average(g => g.NumberOf5000),
                    GameAverage: gameHistory.Count == 0 ? 0 : gameHistory.Average(g => g.Sum),
                    NumberOfGamesPlayed: gameHistory.Count,
                    NumberOfFirstPlaceInGame: gameHistory.Count(gh => gh.Position == 1),
                    NumberOfSecondPlaceInGame: gameHistory.Count(gh => gh.Position == 2),
                    NumberOfThirdPlaceInGame: gameHistory.Count(gh => gh.Position == 3)
                ),
                MapsSummary: mapsSummary,
                GameHistory: gameHistory);
        }

        return null;
    }

    public int CountChallengesNotDone(string id, bool takeAllMaps)
    {
        return _context
            .Challenges
            .WhereIf(!takeAllMaps, c => (c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || c.Map.IsMapForGame))
            .Where(c => !c.PlayerScores.Any(ps => ps.PlayerId == id && ps.PlayerGuesses.Count == 5 && ps.PlayerGuesses.All(g => g.Score != null) && ps.ChallengeId == c.Id))
            .Count();
    }

    public async Task<PlayerResumeDto?> GetResumeAsync(string id, bool takeAllMaps)
    {
        return await _context
            .Players
            .Where(p => p.Id == id)
            .Select(p => new PlayerResumeDto
            (
                p.Id,
                p.Name,
                p.Title,
                p.IconUrl,
                p.GeoGuessrName,
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .SelectMany(p => p.PlayerGuesses)
                    .Count(g => g.Score == 5000),
                p
                    .PlayerScores
                    .Where(p => takeAllMaps || ((p.Challenge.TimeLimit ?? 300) == 300 && (p.Challenge.GameId != -1 || p.Challenge.Map.IsMapForGame)))
                    .Count(p => p.PlayerGuesses.Count == 5 && p.PlayerGuesses.All(g => g.Score != null)),
                p
                    .PlayerScores
                    .Where(ps => ps.Challenge.GameId != -1 && (takeAllMaps || (ps.Challenge.TimeLimit ?? 300) == 300))
                    .Select(ps => new { ps.Challenge.GameId, ps.Sum })
                    .GroupBy(p => p.GameId)
                    .Where(g => g.Count() == 3)
                    .Select(g => new { Id = g.Key, Sum = g.Sum(p => p.Sum) })
                    .OrderByDescending(g => g.Sum)
                    .First()
                    .Sum,
                p
                    .PlayerScores
                    .Where(ps => ps.Challenge.GameId != -1 && (takeAllMaps || (ps.Challenge.TimeLimit ?? 300) == 300))
                    .GroupBy(ps => ps.Challenge.GameId)
                    .Count(g => g.Count() == 3)
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerDto?> GetAsync(string id)
    {
        return await _context
            .Players
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
        ArgumentNullException.ThrowIfNull(dto);

        Player? player = await _context.Players.FindAsync(id);
        if (player is not null)
        {
            player.Name = dto.Name;
            player.AssociatedPlayerId = string.IsNullOrWhiteSpace(dto.AssociatedPlayerId) ? null : dto.AssociatedPlayerId;

            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKeys.PlayerServiceGetAllAsync);
            _cache.Remove(CacheKeys.PlayerServiceGetGamesWithPlayersScoreAsync);
        }

        return await GetAsync(id);
    }

    public async Task<PlayerDto?> UpdateGeoGuessrProfileAsync(string id)
    {
        Player? player = await _context.Players.FindAsync(id);
        if (player is not null)
        {
            Models.GeoGuessrPlayerStatistics? statistics = await _geoGuessrService.GetPlayerStatisticsAsync(id);
            if (statistics is not null)
            {
                player.Title = statistics.LifeTimeXpProgression.CurrentTitle.Name;
                player.LastGeoGuessrUpdate = DateTime.UtcNow;

                _context.Players.Update(player);
                await _context.SaveChangesAsync();
            }
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
            _cache.Remove(CacheKeys.PlayerServiceGetAllAsync);
            _cache.Remove(CacheKeys.PlayerServiceGetGamesWithPlayersScoreAsync);
        }
    }

    internal Task<Dictionary<string, ICollection<PlayerGameDto>>> GetGamesWithPlayersScoreAsync(bool takeAllMaps)
    {
        return _cache.GetOrCreateAsync(CacheKeys.PlayerServiceGetGamesWithPlayersScoreAsync, GetAllFactory)!;

        async Task<Dictionary<string, ICollection<PlayerGameDto>>> GetAllFactory(ICacheEntry entry)
        {
            var gamesWithPlayersScore = await _context.Games
                .Where(g => g.Id != -1 && g.Challenges.Count(c => takeAllMaps || (c.TimeLimit ?? 300) == 300) == 3)
                .Select(g => new
                {
                    g.Id,
                    g.Date,
                    g.Name,
                    Scores = g
                        .Challenges
                        .SelectMany(c => c.PlayerScores)
                        .GroupBy(ps => ps.PlayerId)
                        .Select(scores => new
                        {
                            PlayerId = scores.Key,
                            Sum = scores.Sum(ps => ps.Sum),
                            NumberOf5000 = scores.Sum(ps => ps.PlayerGuesses.Count(g => g.Score == 5000)),
                        })
                })
                .ToListAsync();

            var games = new Dictionary<string, ICollection<PlayerGameDto>>();
            foreach (var game in gamesWithPlayersScore)
            {
                int position = 1;
                foreach (var playerScoreForGame in game.Scores.OrderByDescending(s => s.Sum))
                {
                    if (!games.TryGetValue(playerScoreForGame.PlayerId, out ICollection<PlayerGameDto>? value))
                    {
                        value = [];
                        games[playerScoreForGame.PlayerId] = value;
                    }

                    value.Add(new PlayerGameDto(
                        game.Id,
                        playerScoreForGame.Sum,
                        game.Date,
                        game.Name,
                        playerScoreForGame.NumberOf5000,
                        position));
                    position++;
                }
            }

            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10)).SetSize(games.Count);

            return games;
        }
    }
}
