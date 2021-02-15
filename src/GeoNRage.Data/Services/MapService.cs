using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Data.Services
{
    public class MapService
    {
        private readonly GeoNRageDbContext _context;

        public MapService(GeoNRageDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Map>> GetAllAsync()
        {
            return await _context.Maps.Include(m => m.Games).ToListAsync();
        }

        public async Task<Map> CreateAsync(string name)
        {
            EntityEntry<Map> map = await _context.Maps.AddAsync(new Map
            {
                Name = name,
            });

            await _context.SaveChangesAsync();

            return map.Entity;
        }

        public async Task<Map?> UpdateAsync(int id, string name)
        {
            Map? map = await _context.Maps.FindAsync(id);
            if (map is not null)
            {
                map.Name = name;

                _context.Maps.Update(map);
                await _context.SaveChangesAsync();
            }

            return map;
        }

        public async Task DeleteAsync(int id)
        {
            Map? map = await _context.Maps.Include(m => m.Games).FirstAsync(m => m.Id == id);
            if (map is not null)
            {
                if (map.Games.Count > 0)
                {
                    throw new InvalidOperationException("Cannot delete a map in use.");
                }

                _context.Maps.Remove(map);
                await _context.SaveChangesAsync();
            }
        }
    }
}
