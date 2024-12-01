using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GeoNRage.Server.Services;

[AutoConstructor]
internal sealed partial class MapService
{
    private readonly GeoNRageDbContext _context;
    private readonly IMemoryCache _cache;

    public Task<IEnumerable<MapDto>> GetAllAsync()
    {
        return _cache.GetOrCreateAsync(CacheKeys.MapServiceGetAllAsync, GetAllFactory)!;

        async Task<IEnumerable<MapDto>> GetAllFactory(ICacheEntry entry)
        {
            List<MapDto> maps = await _context
                .Maps
                .OrderBy(m => m.Name)
                .AsNoTracking()
                .Select(m => new MapDto
                (
                    m.Id,
                    m.Name,
                    m.GeoGuessrName,
                    m.IsMapForGame
                ))
                .ToListAsync();

            entry.SetSlidingExpiration(TimeSpan.FromDays(1)).SetSize(maps.Count);

            return maps;
        }
    }

    public Task<MapStatisticsDto?> GetMapStatisticsAsync(string id, bool takeAllMaps)
    {
        return _context
            .Maps
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MapStatisticsDto
            (
                m.Id,
                m.Name,
                m.Challenges.Where(c => takeAllMaps || ((c.TimeLimit ?? 300) == 300 && (c.GameId != -1 || m.IsMapForGame))).SelectMany(c => c
                    .PlayerScores
                    .Where(ps => ps.PlayerGuesses.Count == 5)
                    .Select(ps => new MapScoreDto(ps.Player.Name, ps.PlayerGuesses.Sum(pg => pg.Score) ?? 0, ps.PlayerGuesses.Sum(pg => pg.Time) ?? 0)))
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<MapDto?> UpdateAsync(string id, MapEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));

        Map? map = await _context.Maps.FindAsync(id);
        if (map is not null)
        {
            map.Name = dto.Name;
            map.IsMapForGame = dto.IsMapForGame;

            _context.Maps.Update(map);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKeys.MapServiceGetAllAsync);
        }

        return await GetAsync(id);
    }

    public async Task DeleteAsync(string id)
    {
        Map? map = await _context.Maps.FindAsync(id);
        if (map is not null)
        {
            if (await _context.Challenges.AnyAsync(c => c.MapId == id))
            {
                throw new InvalidOperationException("Cannot delete a map in use.");
            }

            _context.Maps.Remove(map);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKeys.MapServiceGetAllAsync);
        }
    }

    private async Task<MapDto?> GetAsync(string id)
    {
        return await _context
            .Maps
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MapDto
            (
                m.Id,
                m.Name,
                m.GeoGuessrName,
                m.IsMapForGame
            ))
            .FirstOrDefaultAsync();
    }
}
