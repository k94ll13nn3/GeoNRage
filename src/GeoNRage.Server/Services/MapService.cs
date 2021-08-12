using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services;

[AutoConstructor]
public partial class MapService
{
    private readonly GeoNRageDbContext _context;

    public async Task<IEnumerable<MapDto>> GetAllAsync()
    {
        return await _context
            .Maps
            .OrderBy(m => m.Name)
            .AsNoTracking()
            .Select(m => new MapDto
            (
                m.Id,
                m.Name,
                m.IsMapForGame
            ))
            .ToListAsync();
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
                m.IsMapForGame
            ))
            .FirstOrDefaultAsync();
    }
}
