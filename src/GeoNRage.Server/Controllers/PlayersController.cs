using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [AutoConstructor]
    public partial class PlayersController : ControllerBase
    {
        private readonly PlayerService _playerService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            return await _playerService.GetAllAsync();
        }

        [AllowAnonymous]
        [HttpGet("statistics")]
        public async Task<IEnumerable<PlayerStatisticDto>> GetAllStatisticsAsync()
        {
            return await _playerService.GetAllStatisticsAsync(Request.Headers["show-all-maps"] == "True");
        }

        [AllowAnonymous]
        [HttpGet("{id}/full")]
        public async Task<ActionResult<PlayerFullDto>> GetFullAsync(string id)
        {
            PlayerFullDto? player = await _playerService.GetFullAsync(id, Request.Headers["show-all-maps"] == "True");
            return player ?? (ActionResult<PlayerFullDto>)NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, PlayerEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            try
            {
                PlayerDto? updatedPlayer = await _playerService.UpdateAsync(id, dto);
                if (updatedPlayer is null)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            try
            {
                await _playerService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
