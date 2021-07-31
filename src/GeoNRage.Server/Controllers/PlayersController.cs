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
    public partial class PlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PlayerService _playerService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            IEnumerable<Player> players = await _playerService.GetAllAsync(false);
            return _mapper.Map<IEnumerable<Player>, IEnumerable<PlayerDto>>(players);
        }

        [AllowAnonymous]
        [HttpGet("full")]
        public async Task<IEnumerable<PlayerFullDto>> GetAllFullAsync()
        {
            IEnumerable<Player> players = await _playerService.GetAllAsync(true);
            return _mapper.Map<IEnumerable<Player>, IEnumerable<PlayerFullDto>>(players);
        }

        [AllowAnonymous]
        [HttpGet("{id}/full")]
        public async Task<ActionResult<PlayerFullDto>> GetFullAsync(string id)
        {
            Player? player = await _playerService.GetFullAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            return _mapper.Map<Player, PlayerFullDto>(player);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdateAsync(string id, PlayerEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            try
            {
                Player? updatedPlayer = await _playerService.UpdateAsync(id, dto);
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
