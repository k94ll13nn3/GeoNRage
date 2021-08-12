using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services;

[AutoConstructor]
public partial class LocationService
{
    private readonly GeoNRageDbContext _context;

    public async Task<IEnumerable<LocationDto>> GetAllAsync(bool takeAllMaps)
    {
        IQueryable<Location> query = _context.Locations
               .AsNoTracking();

        if (!takeAllMaps)
        {
            query = query.Where(l => (l.Challenge.TimeLimit ?? 300) == 300 && (l.Challenge.GameId != -1 || l.Challenge.Map.IsMapForGame));
        }

        return await query
            .GroupBy(l => new
            {
                l.AdministrativeAreaLevel1,
                l.AdministrativeAreaLevel2,
                l.Latitude,
                l.Longitude,
                l.Locality,
                l.DisplayName,
                l.Country,
            })
            .Select(l => new LocationDto
            (
                l.Key.DisplayName,
                l.Key.Locality,
                l.Key.AdministrativeAreaLevel2,
                l.Key.AdministrativeAreaLevel1,
                l.Key.Country,
                l.Count(),
                l.Key.Latitude,
                l.Key.Longitude
            ))
            .ToListAsync();
    }
}
