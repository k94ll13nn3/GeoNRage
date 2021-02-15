using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Data.Services
{
    public class PlayerService
    {
        private readonly GeoNRageDbContext _context;

        public PlayerService(GeoNRageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }
    }
}
