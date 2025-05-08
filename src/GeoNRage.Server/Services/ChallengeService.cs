using System.Globalization;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Services;

[AutoConstructor]
internal sealed partial class ChallengeService
{
    private readonly GeoNRageDbContext _context;
    private readonly IHttpClientFactory _clientFactory;
    private readonly GeoGuessrService _geoGuessrService;
    private readonly IMemoryCache _cache;

    [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
    private readonly ApplicationOptions _options;

    public IQueryable<ChallengeDto> GetAll(bool onlyMapForGame, string[]? playersToExclude, string? currentPlayerId)
    {
        IQueryable<Challenge> query = _context.Challenges
            .Where(c => c.GameId == -1)
            .WhereIf(onlyMapForGame, c => c.Map.IsMapForGame && (c.TimeLimit ?? 300) == 300)
            .WhereIf(playersToExclude?.Length > 0, c => c.PlayerScores.All(p => !playersToExclude!.Contains(p.PlayerId)));

        return query
            .Select(c => new ChallengeDto
            {
                Id = c.Id,
                MapId = c.Map.Id,
                MapName = c.Map.Name,
                GeoGuessrId = c.GeoGuessrId,
                CreatorName = c.Creator == null ? null : c.Creator.Name,
                MaxScore = c.MaxScore,
                PlayerScore = c.PlayerScores.Where(p => p.PlayerId == currentPlayerId).Select(p => new { p.Sum }).FirstOrDefault()!.Sum,
                CreatedAt = c.CreatedAt
            });
    }

    public async Task<IEnumerable<ChallengeAdminViewDto>> GetAllAsAdminViewAsync()
    {
        return await _context.Challenges
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new ChallengeAdminViewDto
            (
                c.Id,
                c.MapId,
                c.Map.Name,
                c.GeoGuessrId,
                c.GameId,
                c.Game.Name,
                c.UpdatedAt,
                c.Locations.Count == 5
                    && c.TimeLimit != null
                    && c.CreatorId != null
                    && c.CreatedAt != null
                    && c.UpdatedAt != null
                    && c.PlayerScores.All(p => p.PlayerGuesses.All(g => g.Score != null && g.Distance != null && g.Time != null)),
                c.PlayerScores.Count(p => p.PlayerGuesses.Count == 5)
            ))
            .ToListAsync();
    }

    public async Task<ChallengeDetailDto?> GetAsync(int id)
    {
        return await _context
            .Challenges
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ChallengeDetailDto
            (
                c.Id,
                c.Map.Name,
                c.GeoGuessrId,
                c.PlayerScores.Select(p => new ChallengePlayerScoreDto
                (
                    p.PlayerId,
                    p.Player.Name,
                    p.PlayerGuesses.Select(g => new ChallengePlayerGuessDto
                    (
                        g.RoundNumber,
                        g.Score,
                        g.Time,
                        g.Distance
                    ))
                ))
            ))
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Challenge? challenge = await _context.Challenges.FindAsync(id);
        if (challenge is not null)
        {
            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync();
        }
    }

    public Task<int> ImportChallengeAsync(ChallengeImportDto dto)
    {
        return ImportChallengeAsync(dto, -1);
    }

    public async Task<int> ImportChallengeAsync(ChallengeImportDto dto, int gameId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        (GeoGuessrChallenge challenge, IList<GeoGuessrChallengeResult> results) = await _geoGuessrService.ImportChallengeAsync(dto.GeoGuessrId);
        var locations = new List<Location>();
        HttpClient googleClient = _clientFactory.CreateClient("google");

        DateOnly creationDate = DateOnly.FromDateTime(results[0].Game.Rounds[0].StartTime.DateTime);
        for (int i = 0; i < results[0].Game.Rounds.Count; i++)
        {
            GeoGuessrRound round = results[0].Game.Rounds[i];
            string query = $"geocode/json?latlng={round.Lat.ToString(CultureInfo.InvariantCulture)},{round.Lng.ToString(CultureInfo.InvariantCulture)}&key={_options.GoogleApiKey}&language=fr";
            GoogleGeocode? geocode = await googleClient.GetFromJsonAsync<GoogleGeocode>(query);
            if (geocode?.Results?.Count > 0)
            {
                GoogleGeocodeResult result = geocode.Results[0];
                var location = new Location()
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
        foreach (GeoGuessrPlayer geoChallengeGamePlayer in results.Select(g => g.Game.Player))
        {
            bool isMainPlayer = true;
            Player player = await _context.Players.FindAsync(geoChallengeGamePlayer.Id) ?? new Player
            {
                Id = geoChallengeGamePlayer.Id,
                Name = geoChallengeGamePlayer.Nick,
            };

            if (player.AssociatedPlayerId is not null)
            {
                player = (await _context.Players.FindAsync(player.AssociatedPlayerId))!;
                isMainPlayer = false;
            }

            // Update icon in order to have the most recent one.
            if (!string.IsNullOrWhiteSpace(geoChallengeGamePlayer.Pin.Url.ToString()))
            {
                player.IconUrl = new Uri($"https://www.geoguessr.com/images/resize:auto:144:144/gravity:ce/plain/{geoChallengeGamePlayer.Pin.Url}");
            }
            else
            {
                player.IconUrl = Constants.BaseAvatarUrl;
            }

            // Update new player or existing player name from GeoGuessr
            player.GeoGuessrName = geoChallengeGamePlayer.Nick;

            var playerScore = new PlayerScore
            {
                PlayerId = player.Id,
                Player = player,
                PlayerGuesses = [.. geoChallengeGamePlayer.Guesses.Select((p, i) => new PlayerGuess
                {
                    Score = p.RoundScoreInPoints,
                    RoundNumber = i + 1,
                    Time = p.Time,
                    TimedOut = p.TimedOut,
                    TimedOutWithGuess = p.TimedOutWithGuess,
                    Distance = p.DistanceInMeters,
                    Steps = p.StepsCount
                })],
                Sum = geoChallengeGamePlayer.Guesses.Sum(p => p.RoundScoreInPoints)
            };

            if (isMainPlayer && playerScores.Exists(ps => ps.PlayerId == playerScore.PlayerId))
            {
                playerScores.RemoveAll(ps => ps.PlayerId == playerScore.PlayerId);
            }

            if (!playerScores.Exists(ps => ps.PlayerId == playerScore.PlayerId))
            {
                playerScores.Add(playerScore);
            }
        }

        Map map = await _context.Maps.FindAsync(challenge.Map.Id) ?? new Map
        {
            Id = challenge.Map.Id,
            Name = challenge.Map.Name,
        };

        // Update new map or existing map name from GeoGuessr
        map.GeoGuessrName = challenge.Map.Name;

        Player? creator = await _context.Players.FindAsync(challenge.Creator.Id);
        if (creator is null)
        {
            throw new InvalidOperationException($"Cannot import challenges created by {challenge.Creator.Nick}.");
        }

        var newChallenge = new Challenge
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
            CreatedAt = creationDate,
            MaxScore = playerScores.Max(ps => ps.Sum),
        };

        Challenge? existingChallenge = await _context
            .Challenges
            .Include(c => c.Map)
            .Include(c => c.PlayerScores).ThenInclude(c => c.PlayerGuesses)
            .Include(c => c.Locations)
            .SingleOrDefaultAsync(c => c.GeoGuessrId == dto.GeoGuessrId);
        int challengeId;
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
            existingChallenge.CreatedAt = newChallenge.CreatedAt;
            existingChallenge.MaxScore = newChallenge.MaxScore;

            challengeId = existingChallenge.Id;
            await _context.SaveChangesAsync();
        }
        else
        {
            EntityEntry<Challenge>? newChallengeEntity = await _context.Challenges.AddAsync(newChallenge);
            await _context.SaveChangesAsync();
            challengeId = newChallengeEntity.Entity.Id;
        }

        _cache.Remove(CacheKeys.PlayerServiceGetAllAsync);
        _cache.Remove(CacheKeys.MapServiceGetAllAsync);
        _cache.Remove(CacheKeys.PlayerServiceGetGamesWithPlayersScoreAsync);
        return challengeId;
    }
}
