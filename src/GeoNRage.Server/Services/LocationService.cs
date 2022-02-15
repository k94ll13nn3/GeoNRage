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

        return (await query
            .GroupBy(l => new
            {
                l.Latitude,
                l.Longitude,
            })
            .Select(l => new
            {
                location = l.First(),
                count = l.Count(),
                l.Key.Latitude,
                l.Key.Longitude
            })
            .ToListAsync())
            .ConvertAll(l => new LocationDto
            (
                l.location.DisplayName,
                l.location.Locality,
                l.location.AdministrativeAreaLevel2,
                l.location.AdministrativeAreaLevel1,
                l.location.Country,
                l.count,
                l.Latitude,
                l.Longitude
            ));
    }
}
