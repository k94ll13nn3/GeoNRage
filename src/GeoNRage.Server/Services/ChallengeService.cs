using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Server.GeoGuessr;
using GeoNRage.Server.Models;
using GeoNRage.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class ChallengeService
    {
        private readonly GeoNRageDbContext _context;
        private readonly IHttpClientFactory _clientFactory;

        [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
        private readonly ApplicationOptions _options;

        private readonly GeoGuessrService _geoGuessrService;

        public async Task<IEnumerable<Challenge>> GetAllAsync()
        {
            return await _context.Challenges
                .Include(c => c.Map)
                .Include(c => c.Game)
                .Include(c => c.PlayerScores).ThenInclude(p => p.Player)
                .Include(c => c.Locations)
                .Include(c => c.Creator)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Challenge>> GetAllWithoutGameAsync()
        {
            return await _context.Challenges
                .Include(c => c.Map)
                .Include(c => c.Game)
                .Include(c => c.PlayerScores).ThenInclude(p => p.Player)
                .Include(c => c.Locations)
                .Include(c => c.Creator)
                .Where(c => c.GameId == int.MaxValue)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Challenge?> GetAsync(int id)
        {
            return await _context
                .Challenges
                .Include(c => c.Map)
                .Include(c => c.Game)
                .Include(c => c.PlayerScores).ThenInclude(p => p.Player)
                .Include(c => c.Locations)
                .Include(c => c.Creator)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            Challenge? challenge = await _context.Challenges
                .Include(c => c.PlayerScores)
                .ThenInclude(p => p.Player)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (challenge is not null)
            {
                _context.Challenges.Remove(challenge);
                await _context.SaveChangesAsync();
            }
        }

        public Task ImportChallengeAsync(ChallengeImportDto dto)
        {
            return ImportChallengeAsync(dto, int.MaxValue);
        }

        public async Task ImportChallengeAsync(ChallengeImportDto dto, int gameId)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            (GeoGuessrChallenge challenge, IList<GeoGuessrChallengeResult> results) = await _geoGuessrService.ImportChallengeAsync(dto.GeoGuessrId);
            List<Location> locations = new();
            HttpClient googleClient = _clientFactory.CreateClient("google");
            for (int i = 0; i < results[0].Game.Rounds.Count; i++)
            {
                GeoGuessrRound round = results[0].Game.Rounds[i];
                string query = $"geocode/json?latlng={round.Lat.ToString(CultureInfo.InvariantCulture)},{round.Lng.ToString(CultureInfo.InvariantCulture)}&key={_options.GoogleApiKey}";
                GoogleGeocode? geocode = await googleClient.GetFromJsonAsync<GoogleGeocode>(query);
                if (geocode?.Results?.Count > 0)
                {
                    GoogleGeocodeResult result = geocode.Results[0];
                    Location location = new()
                    {
                        DisplayName = result.FormattedAddress,
                        Locality = result.AddressComponents.FirstOrDefault(x => x.Types.Contains("locality"))?.Name,
                        AdministrativeAreaLevel2 = result.AddressComponents.FirstOrDefault(x => x.Types.Contains("administrative_area_level_2"))?.Name,
                        AdministrativeAreaLevel1 = result.AddressComponents.FirstOrDefault(x => x.Types.Contains("administrative_area_level_1"))?.Name,
                        Country = result.AddressComponents.FirstOrDefault(x => x.Types.Contains("country"))?.Name,
                        Latitude = round.Lat,
                        Longitude = round.Lng,
                        RoundNumber = i + 1,
                    };

                    locations.Add(location);
                }
            }

            var playerScores = new List<PlayerScore>();
            foreach (GeoGuessrChallengeResult geoChallenge in results)
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

            Map map = await _context.Maps.FindAsync(challenge.Map.Id) ?? new Map
            {
                Id = challenge.Map.Id,
                Name = challenge.Map.Name,
            };

            Player? creator = await _context.Players.FindAsync(challenge.Creator.Id);
            if (creator is null)
            {
                throw new InvalidOperationException($"Cannot import challenges created by {challenge.Creator.Nick}.");
            }

            Challenge newChallenge = new()
            {
                GameId = gameId,
                MapId = map.Id,
                Map = map,
                GeoGuessrId = dto.GeoGuessrId,
                PlayerScores = playerScores,
                TimeLimit = challenge.Challenge.TimeLimit,
                Locations = locations,
                CreatorId = creator.Id,
                UpdatedAt = DateTime.UtcNow,
            };

            Challenge? existingChallenge = await _context
                .Challenges
                .Include(c => c.Map)
                .Include(c => c.Game)
                .Include(c => c.PlayerScores).ThenInclude(p => p.Player)
                .Include(c => c.Locations)
                .SingleOrDefaultAsync(c => c.GeoGuessrId == dto.GeoGuessrId);
            if (existingChallenge is not null)
            {
                if (existingChallenge.GameId != gameId)
                {
                    throw new InvalidOperationException($"The challenge with GeoGuessr Id '{dto.GeoGuessrId}' is already linked to a game and cannot be updated.");
                }

                if (!dto.OverrideData)
                {
                    throw new InvalidOperationException($"The challenge with GeoGuessr Id '{dto.GeoGuessrId}' already exists.");
                }

                existingChallenge.PlayerScores = newChallenge.PlayerScores;
                existingChallenge.MapId = newChallenge.MapId;
                existingChallenge.Map = newChallenge.Map;
                existingChallenge.TimeLimit = newChallenge.TimeLimit;
                existingChallenge.Locations = newChallenge.Locations;
                existingChallenge.CreatorId = newChallenge.CreatorId;
                existingChallenge.UpdatedAt = newChallenge.UpdatedAt;
            }
            else
            {
                _context.Challenges.Add(newChallenge);
            }

            await _context.SaveChangesAsync();
        }
    }
}
