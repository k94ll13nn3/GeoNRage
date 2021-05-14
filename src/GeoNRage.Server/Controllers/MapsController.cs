using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [AutoConstructor]
    public partial class MapsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly MapService _mapService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<MapDto>> GetAllAsync()
        {
            IEnumerable<Map> maps = await _mapService.GetAllAsync();

            return _mapper.Map<IEnumerable<Map>, IEnumerable<MapDto>>(maps);
        }

        [HttpPost]
        public async Task<MapDto> PostAsync(MapCreateDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            Map createdMap = await _mapService.CreateAsync(dto);
            return _mapper.Map<Map, MapDto>(createdMap);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<MapDto>> UpdateAsync(string id, MapEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            Map? updatedMap = await _mapService.UpdateAsync(id, dto);
            if (updatedMap == null)
            {
                return NotFound();
            }

            return _mapper.Map<Map, MapDto>(updatedMap);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _mapService.DeleteAsync(id);
            return NoContent();
        }
    }
}
