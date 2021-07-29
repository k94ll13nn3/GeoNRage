using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly MapService _mapService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<MapDto>> GetAllAsync()
        {
            return await _mapService.GetAllAsync();
        }

        [HttpPost]
        public async Task<ActionResult<MapDto>> PostAsync(MapCreateDto dto)
        {
            try
            {
                _ = dto ?? throw new ArgumentNullException(nameof(dto));
                return await _mapService.CreateAsync(dto);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<MapDto>> UpdateAsync(string id, MapEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            MapDto? updatedMap = await _mapService.UpdateAsync(id, dto);
            return updatedMap ?? (ActionResult<MapDto>)NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _mapService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
