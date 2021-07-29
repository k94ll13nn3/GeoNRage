using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoNRage.Server.Services
{
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
                .Select(m => CreateDto(m))
                .ToListAsync();
        }

        public async Task<MapDto> CreateAsync(MapCreateDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            if (await _context.Maps.AnyAsync(m => m.Id == dto.Id))
            {
                throw new InvalidOperationException($"Map '{dto.Id}' already exists.");
            }

            EntityEntry<Map> map = await _context.Maps.AddAsync(new Map
            {
                Id = dto.Id,
                Name = dto.Name,
                IsMapForGame = dto.IsMapForGame,
            });

            await _context.SaveChangesAsync();

            return (await GetAsync(map.Entity.Id))!;
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
                .Select(m => CreateDto(m))
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        private static MapDto CreateDto(Map map)
        {
            return new MapDto
            {
                Id = map.Id,
                IsMapForGame = map.IsMapForGame,
                Name = map.Name,
            };
        }
    }
}
