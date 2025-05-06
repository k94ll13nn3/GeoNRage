using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Server.Services;

[AutoConstructor]
internal sealed partial class GameService
{
    private readonly GeoNRageDbContext _context;
    private readonly ChallengeService _challengeService;

    public IQueryable<GameDto> GetAll()
    {
        return _context
            .Games
            .OrderByDescending(g => g.Date)
            .Where(g => g.Id != -1)
            .Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Date = g.Date
            });
    }

    public async Task<IEnumerable<GameAdminViewDto>> GetAllAsAdminViewAsync()
    {
        return await _context.Games
            .AsNoTracking()
            .OrderByDescending(g => g.Date)
            .Where(g => g.Id != -1)
            .Select(g => new GameAdminViewDto
            (
                g.Id,
                g.Name,
                g.Date,
                g.Challenges.Select(c => new GameChallengeInfoDto
                (
                    c.Id,
                    c.MapId,
                    c.GeoGuessrId
                )),
                g.Challenges.SelectMany(c => c.PlayerScores).Select(p => p.PlayerId).Distinct()
            ))
            .ToListAsync();
    }

    public async Task<GameDetailDto?> GetAsync(int id)
    {
        GameDetailDto? game = await _context
            .Games
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GameDetailDto
            (
                g.Id,
                g.Name,
                g.Date,
                g.Challenges.Select(c => new GameChallengeDto
                (
                    c.Id,
                    c.MapId,
                    c.GeoGuessrId,
                    c.Map.Name,
                    c.PlayerScores.Select(p => new GameChallengePlayerScoreDto
                    (
                        p.PlayerId,
                        p.Player.Name,
                        p.Player.IconUrl,
                        p.PlayerGuesses.Any(g => g.RoundNumber == 1) ? p.PlayerGuesses.First(g => g.RoundNumber == 1).Score : null,
                        p.PlayerGuesses.Any(g => g.RoundNumber == 2) ? p.PlayerGuesses.First(g => g.RoundNumber == 2).Score : null,
                        p.PlayerGuesses.Any(g => g.RoundNumber == 3) ? p.PlayerGuesses.First(g => g.RoundNumber == 3).Score : null,
                        p.PlayerGuesses.Any(g => g.RoundNumber == 4) ? p.PlayerGuesses.First(g => g.RoundNumber == 4).Score : null,
                        p.PlayerGuesses.Any(g => g.RoundNumber == 5) ? p.PlayerGuesses.First(g => g.RoundNumber == 5).Score : null,
                        p.PlayerGuesses.Sum(g => g.Score)
                    ))
                )),
                g.Challenges.SelectMany(c => c.PlayerScores).Select(p => new GamePlayerDto
                (
                    p.PlayerId,
                    p.Player.Name
                )).Distinct(),
                null,
                null
            ))
            .FirstOrDefaultAsync();

        if (game is not null)
        {
            var previousGame = await _context.Games
                .Where(g => g.Date < game.Date && g.Id != -1)
                .OrderByDescending(g => g.Date)
                .Select(g => new { g.Id })
                .FirstOrDefaultAsync();

            var nextGame = await _context.Games
                .Where(g => g.Date > game.Date && g.Id != -1)
                .OrderBy(g => g.Date)
                .Select(g => new { g.Id })
                .FirstOrDefaultAsync();

            game = game with { PreviousGameId = previousGame?.Id, NextGameId = nextGame?.Id };
        }

        return game;
    }

    public bool Exists(int gameId)
    {
        return _context
            .Games
            .AsNoTracking()
            .Any(g => g.Id == gameId);
    }

    public async Task<int> CreateAsync(GameCreateOrEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));

        IEnumerable<string> geoGuessrIds = dto.Challenges.Select(c => c.GeoGuessrId);
        if (_context.Challenges.Select(c => c.GeoGuessrId).AsEnumerable().Intersect(geoGuessrIds).Any())
        {
            throw new InvalidOperationException("One or more challenge GeoGuessr ids are already registered.");
        }

        if (dto.Challenges.GroupBy(x => x.GeoGuessrId).Any(g => g.Count() > 1))
        {
            throw new InvalidOperationException("Two or more challenge GeoGuessr ids are the same.");
        }

        IEnumerable<string> mapIds = dto.Challenges.Select(c => c.MapId);
        if (mapIds.Except(_context.Maps.Select(c => c.Id).AsEnumerable()).Any())
        {
            throw new InvalidOperationException("One or more maps do not exists.");
        }

        IEnumerable<string> playerIds = dto.PlayerIds;
        if (playerIds.Except(_context.Players.Select(c => c.Id).AsEnumerable()).Any())
        {
            throw new InvalidOperationException("One or more players do not exists.");
        }

        EntityEntry<Game> game = await _context.Games.AddAsync(new Game
        {
            Name = dto.Name,
            Date = dto.Date,
            Challenges = [.. dto.Challenges.Select(x => new Challenge
            {
                GeoGuessrId = x.GeoGuessrId,
                MapId = x.MapId,
                PlayerScores = [.. dto.PlayerIds.Select(p => new PlayerScore { PlayerId = p })]
            })]
        });
        await _context.SaveChangesAsync();

        return game.Entity.Id;
    }

    public async Task<Game?> UpdateAsync(int id, GameCreateOrEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));

        if (!Exists(id))
        {
            return null;
        }

        Game game = await GetInternalAsync(id, true);
        IEnumerable<string> mapIds = dto.Challenges.Select(c => c.MapId);
        if (mapIds.Except(_context.Maps.Select(c => c.Id).AsEnumerable()).Any())
        {
            throw new InvalidOperationException("One or more maps do not exists.");
        }

        if (dto.Challenges.GroupBy(x => x.GeoGuessrId).Any(g => g.Count() > 1))
        {
            throw new InvalidOperationException("Two or more challenge GeoGuessr ids are the same.");
        }

        // Check challenge GeoGuessr id for new challenges only.
        IEnumerable<string> geoGuessrIds = dto.Challenges.Where(c => c.Id == 0).Select(c => c.GeoGuessrId);
        if (_context.Challenges.Select(c => c.GeoGuessrId).AsEnumerable().Intersect(geoGuessrIds).Any())
        {
            throw new InvalidOperationException("One or more challenge GeoGuessr ids are already registered.");
        }

        IEnumerable<string> playerIds = dto.PlayerIds;
        if (playerIds.Except(_context.Players.Select(c => c.Id).AsEnumerable()).Any())
        {
            throw new InvalidOperationException("One or more players do not exists.");
        }

        game.Name = dto.Name;
        game.Date = dto.Date;

        IEnumerable<int> challengeIds = dto.Challenges.Select(x => x.Id).Where(id => id != 0);
        game.Challenges =
        [
            .. game.Challenges
                        .Where(c => challengeIds.Contains(c.Id))
,
            .. dto.Challenges.Where(c => c.Id == 0).Select(x => new Challenge
                {
                    GeoGuessrId = x.GeoGuessrId,
                    MapId = x.MapId,
                    PlayerScores = [.. dto.PlayerIds.Select(p => new PlayerScore { PlayerId = p })]
                }),
        ];

        foreach (Challenge challenge in game.Challenges)
        {
            GameChallengeCreateOrEditDto? modifiedChallenge = dto.Challenges.FirstOrDefault(c => c.Id == challenge.Id && challenge.Id != 0);
            if (modifiedChallenge is not null)
            {
                challenge.MapId = modifiedChallenge.MapId;
                challenge.GeoGuessrId = modifiedChallenge.GeoGuessrId;
            }

            challenge.PlayerScores = [.. challenge.PlayerScores.Where(ps => dto.PlayerIds.Contains(ps.PlayerId))];
            challenge.PlayerScores =
            [
                .. challenge.PlayerScores,
                .. dto.PlayerIds.Where(id => !challenge.PlayerScores.Any(ps => ps.PlayerId == id)).Select(p => new PlayerScore { PlayerId = p }),
            ];
        }

        _context.Games.Update(game);
        await _context.SaveChangesAsync();
        return await GetInternalAsync(game.Id, false);
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

    public async Task AddPlayerAsync(int gameId, string playerId)
    {
        if (Exists(gameId))
        {
            Game game = await GetInternalAsync(gameId, true);
            if (!await _context.Players.AnyAsync(p => p.Id == playerId))
            {
                throw new InvalidOperationException($"No player with id '{playerId}' exists");
            }

            foreach (ICollection<PlayerScore> playerScores in game.Challenges.Select(c => c.PlayerScores))
            {
                if (!playerScores.Select(p => p.PlayerId).Contains(playerId))
                {
                    playerScores.Add(new PlayerScore { PlayerId = playerId });
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int newScore)
    {
        if (Exists(gameId))
        {
            Game? game = await GetInternalAsync(gameId, true);
            PlayerScore playerScore = game.Challenges.First(c => c.Id == challengeId).PlayerScores.First(p => p.PlayerId == playerId);
            PlayerGuess? existingRound = playerScore.PlayerGuesses.SingleOrDefault(g => g.RoundNumber == round);
            if (existingRound is not null)
            {
                existingRound.Score = newScore;
            }
            else
            {
                playerScore.PlayerGuesses.Add(new() { Score = newScore, RoundNumber = round });
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task ImportChallengeAsync(int id)
    {
        Game gameForDto = await GetInternalAsync(id, false);
        foreach (Challenge challenge in gameForDto.Challenges)
        {
            await _challengeService.ImportChallengeAsync(new() { GeoGuessrId = challenge.GeoGuessrId, OverrideData = true }, id);
        }

        // Update the dto with the updated challenges
        gameForDto = await GetInternalAsync(id, false);
        var editDto = new GameCreateOrEditDto
        {
            Name = gameForDto.Name,
            Date = gameForDto.Date,
            PlayerIds = [.. gameForDto.Challenges.SelectMany(c => c.PlayerScores).Select(p => p.PlayerId).Distinct()],
            Challenges = [.. gameForDto.Challenges.Select(c => new GameChallengeCreateOrEditDto
            {
                Id = c.Id,
                MapId = c.MapId,
                GeoGuessrId = c.GeoGuessrId
            })]
        };

        await UpdateAsync(id, editDto);
    }

    private async Task<Game> GetInternalAsync(int id, bool tracking)
    {
        IQueryable<Game> query = _context
            .Games
            .Include(g => g.Challenges).ThenInclude(c => c.Map)
            .Include(g => g.Challenges).ThenInclude(c => c.PlayerScores).ThenInclude(c => c.Player)
            .Include(g => g.Challenges).ThenInclude(c => c.PlayerScores).ThenInclude(c => c.PlayerGuesses);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        Game? game = await query.FirstOrDefaultAsync(g => g.Id != -1 && g.Id == id);
        if (game is null)
        {
            throw new InvalidOperationException($"Game with id {id} does not exists?");
        }

        return game;
    }
}
