using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Services
{
    [AutoConstructor]
    public partial class ChallengeService
    {
        private readonly GeoNRageDbContext _context;

        public async Task<IEnumerable<Challenge>> GetAllAsync()
        {
            return await _context.Challenges
                .Include(c => c.Map)
                .Include(c => c.Game)
                .Include(c => c.PlayerScores).ThenInclude(p => p.Player)
                .ToListAsync();
        }
    }
}
