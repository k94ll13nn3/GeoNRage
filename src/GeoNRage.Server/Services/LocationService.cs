using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Locations;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class LocationService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<LocationDto>> GetAllAsync()
        {
            return await _context.Locations
                .AsNoTracking()
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
}
