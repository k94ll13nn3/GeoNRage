using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GeoNRage.Server.Dtos.GeoGuessr;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class GameService
    {
        private readonly GeoNRageDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
        private readonly ApplicationOptions _options;

        private readonly CookieContainer _cookieContainer;

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

            return game.Entity;
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

            Game? game = await GetAsync(id);
            if (game is not null)
            {
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
                    ChallengeCreateOrEditDto? modifiedChallenge = dto.Challenges.FirstOrDefault(c => c.Id == challenge.Id && challenge.Id != 0);
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
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Game?> AddPlayerAsync(int gameId, string playerId)
        {
            Game? game = await GetAsync(gameId);
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

            return game;
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

        public async Task<Challenge?> ImportChallengeAsync(int id, ChallengeImportDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            Game? game = await GetAsync(id);
            if (game == null)
            {
                return null;
            }

            HttpClient client = _clientFactory.CreateClient("geoguessr");

            if (_cookieContainer.Count == 0 || _cookieContainer.GetCookies(new Uri("https://www.geoguessr.com")).FirstOrDefault()?.Expired == true)
            {
                await client.PostAsJsonAsync("accounts/signin", new GeoGuessrLogin { Email = _options.GeoGuessrEmail, Password = _options.GeoGuessrPassword });
            }

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            GeoGuessrChallenge[]? response = await client.GetFromJsonAsync<GeoGuessrChallenge[]>($"results/scores/{dto.GeoGuessrId}/0/26", options);

            if (response is null)
            {
                throw new InvalidOperationException("Cannot import data.");
            }

            var playerScores = new List<PlayerScore>();
            foreach (GeoGuessrChallenge geoChallenge in response)
            {
                Player player = await _context.Players.FindAsync(geoChallenge.Game.Player.Id) ?? new Player
                {
                    Id = geoChallenge.Game.Player.Id,
                    Name = geoChallenge.Game.Player.Nick,
                };

                var playerScore = new PlayerScore
                {
                    PlayerId = player.Id,
                    Player = player,
                    Round1 = geoChallenge.Game.Player.Guesses[0].RoundScore.Amount,
                    Round2 = geoChallenge.Game.Player.Guesses[1].RoundScore.Amount,
                    Round3 = geoChallenge.Game.Player.Guesses[2].RoundScore.Amount,
                    Round4 = geoChallenge.Game.Player.Guesses[3].RoundScore.Amount,
                    Round5 = geoChallenge.Game.Player.Guesses[4].RoundScore.Amount
                };
                playerScores.Add(playerScore);
            }

            Map map = await _context.Maps.FindAsync(response[0].Game.Map) ?? new Map
            {
                Id = response[0].Game.Map,
                Name = response[0].Game.MapName,
            };

            var challenge = new Challenge
            {
                GameId = id,
                MapId = map.Id,
                Map = map,
                GeoGuessrId = dto.GeoGuessrId,
                PlayerScores = playerScores
            };
            if (dto.PersistData)
            {
                Challenge? existingChallenge = await _context.Challenges.SingleOrDefaultAsync(c => c.GeoGuessrId == dto.GeoGuessrId);
                if (dto.OverrideData)
                {
                    if (existingChallenge is not null)
                    {
                        if (existingChallenge.GameId != id)
                        {
                            throw new InvalidOperationException($"The challenge with GeoGuessr Id '{dto.GeoGuessrId}' is not associated to game '{id}'.");
                        }
                        existingChallenge.PlayerScores = challenge.PlayerScores;
                        existingChallenge.MapId = challenge.MapId;
                    }
                    else
                    {
                        game.Challenges.Add(challenge);
                    }
                }
                else
                {
                    if (existingChallenge is not null)
                    {
                        throw new InvalidOperationException($"The challenge with GeoGuessr Id '{dto.GeoGuessrId}' already exists.");
                    }

                    game.Challenges.Add(challenge);
                }

                await _context.SaveChangesAsync();

                game = await GetAsync(id);
                var createDto = new GameCreateOrEditDto
                {
                    Name = game!.Name,
                    Date = game.Date,
                    Challenges = game.Challenges.Select(c => new ChallengeCreateOrEditDto { Id = c.Id, GeoGuessrId = c.GeoGuessrId, MapId = c.MapId }).ToList(),
                    PlayerIds = game.Challenges.SelectMany(c => c.PlayerScores).Select(p => p.PlayerId).Distinct().ToList()
                };

                await UpdateAsync(id, createDto);
            }

            // Cutting the relations to avoid cycles in JSON.
            challenge.Game = null!;
            foreach (PlayerScore score in challenge.PlayerScores)
            {
                score.Challenge = null!;
            }

            return challenge;
        }
    }
}
