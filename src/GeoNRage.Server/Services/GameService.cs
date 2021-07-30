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
        private readonly ChallengeService _challengeService;

        public async Task<IEnumerable<Game>> GetAllAsync(bool includeNavigation)
        {
            IQueryable<Game> query = _context.Games.OrderByDescending(g => g.Date).Where(g => g.Id != -1);
            if (includeNavigation)
            {
                query = query
                    .Include(g => g.Challenges)
                    .ThenInclude(c => c.Map)
                    .Include(g => g.Challenges)
                    .ThenInclude(c => c.PlayerScores)
                    .ThenInclude(c => c.Player);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public Task<Game?> GetAsync(int id)
        {
            return GetInternalAsync(id, false);
        }

        public async Task<Game> CreateAsync(GameCreateOrEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            IEnumerable<string> geoGuessrIds = dto.Challenges.Select(c => c.GeoGuessrId);
            if (_context.Challenges.Select(c => c.GeoGuessrId).AsEnumerable().Intersect(geoGuessrIds).Any())
            {
                throw new InvalidOperationException("One or more challenge GeoGuessr ids are already registered.");
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
                CreationDate = DateTime.UtcNow,
                Challenges = dto.Challenges.Select(x => new Challenge
                {
                    GeoGuessrId = x.GeoGuessrId,
                    MapId = x.MapId,
                    PlayerScores = dto.PlayerIds.Select(p => new PlayerScore { PlayerId = p }).ToList()
                }).ToList()
            });
            await _context.SaveChangesAsync();

            return (await GetInternalAsync(game.Entity.Id, false))!;
        }

        public async Task<Game?> UpdateAsync(int id, GameCreateOrEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            IEnumerable<string> mapIds = dto.Challenges.Select(c => c.MapId);
            if (mapIds.Except(_context.Maps.Select(c => c.Id).AsEnumerable()).Any())
            {
                throw new InvalidOperationException("One or more maps do not exists.");
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

            Game? game = await GetInternalAsync(id, true);
            if (game is null)
            {
                return null;
            }

            game.Name = dto.Name;
            game.Date = dto.Date;

            IEnumerable<int> challengeIds = dto.Challenges.Select(x => x.Id).Where(id => id != 0);
            game.Challenges = game.Challenges
                .Where(c => challengeIds.Contains(c.Id))
                .Concat(dto.Challenges.Where(c => c.Id == 0).Select(x => new Challenge
                {
                    GeoGuessrId = x.GeoGuessrId,
                    MapId = x.MapId,
                    PlayerScores = dto.PlayerIds.Select(p => new PlayerScore { PlayerId = p }).ToList()
                })).ToList();

            foreach (Challenge challenge in game.Challenges)
            {
                GameChallengeCreateOrEditDto? modifiedChallenge = dto.Challenges.FirstOrDefault(c => c.Id == challenge.Id && challenge.Id != 0);
                if (modifiedChallenge is not null)
                {
                    challenge.MapId = modifiedChallenge.MapId;
                    challenge.GeoGuessrId = modifiedChallenge.GeoGuessrId;
                }

                challenge.PlayerScores = challenge.PlayerScores.Where(ps => dto.PlayerIds.Contains(ps.PlayerId)).ToList();
                challenge.PlayerScores = challenge.PlayerScores.Concat(dto.PlayerIds.Where(id => !challenge.PlayerScores.Any(ps => ps.PlayerId == id)).Select(p => new PlayerScore { PlayerId = p })).ToList();
            }

            _context.Games.Update(game);
            await _context.SaveChangesAsync();
            return (await GetInternalAsync(game.Id, false))!;
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

        public async Task<Game?> AddPlayerAsync(int gameId, string playerId)
        {
            Game? game = await GetInternalAsync(gameId, true);
            if (game is not null)
            {
                if (!await _context.Players.AnyAsync(p => p.Id == playerId))
                {
                    throw new InvalidOperationException($"No player with id '{playerId}' exists");
                }

                foreach (Challenge challenge in game.Challenges)
                {
                    if (!challenge.PlayerScores.Select(p => p.PlayerId).Contains(playerId))
                    {
                        challenge.PlayerScores.Add(new PlayerScore { PlayerId = playerId });
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return await GetInternalAsync(gameId, false);
        }

        public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int newScore)
        {
            Game? game = await GetInternalAsync(gameId, true);
            if (game is not null)
            {
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
            Game gameForDto = (await GetInternalAsync(id, false))!;
            foreach (Challenge challenge in gameForDto.Challenges)
            {
                await _challengeService.ImportChallengeAsync(new() { GeoGuessrId = challenge.GeoGuessrId, OverrideData = true }, id);
            }

            var editDto = new GameCreateOrEditDto
            {
                Name = gameForDto.Name,
                Date = gameForDto.Date,
                Challenges = gameForDto.Challenges.Select(c => new GameChallengeCreateOrEditDto { Id = c.Id, GeoGuessrId = c.GeoGuessrId, MapId = c.MapId }).ToList(),
                PlayerIds = gameForDto.Challenges.SelectMany(c => c.PlayerScores).Select(p => p.PlayerId).Distinct().ToList()
            };

            await UpdateAsync(id, editDto);
        }

        private async Task<Game?> GetInternalAsync(int id, bool tracking)
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

            return await query
                .FirstOrDefaultAsync(g => g.Id != -1 && g.Id == id);
        }
    }
}
