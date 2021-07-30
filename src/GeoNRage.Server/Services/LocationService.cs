using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos;
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
                {
                    AdministrativeAreaLevel1 = l.Key.AdministrativeAreaLevel1,
                    AdministrativeAreaLevel2 = l.Key.AdministrativeAreaLevel2,
                    Country = l.Key.Country,
                    DisplayName = l.Key.DisplayName,
                    Latitude = l.Key.Latitude,
                    Longitude = l.Key.Longitude,
                    Locality = l.Key.Locality,
                    TimesSeen = l.Count()
                })
                .ToListAsync();
        }
    }
}
