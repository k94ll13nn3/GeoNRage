using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos.Auth;
using GeoNRage.Shared.Dtos.Maps;
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

        [Authorize(Roles = Roles.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<MapDto>> UpdateAsync(string id, MapEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            try
            {
                MapDto? updatedMap = await _mapService.UpdateAsync(id, dto);
                if (updatedMap is null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = Roles.SuperAdmin)]
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
