using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class LocationService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
