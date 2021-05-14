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
            IEnumerable<Player> players = await _playerService.GetAllAsync();

            return _mapper.Map<IEnumerable<Player>, IEnumerable<PlayerDto>>(players);
        }

        [HttpPost]
        public async Task<PlayerDto> PostAsync(PlayerCreateDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            Player createdPlayer = await _playerService.CreateAsync(dto);
            return _mapper.Map<Player, PlayerDto>(createdPlayer);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdateAsync(string id, PlayerEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            Player? updatedPlayer = await _playerService.UpdateAsync(id, dto);
            if (updatedPlayer == null)
            {
                return NotFound();
            }

            return _mapper.Map<Player, PlayerDto>(updatedPlayer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _playerService.DeleteAsync(id);
            return NoContent();
        }
    }
}
