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
    public class PlayersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PlayerService _playerService;

        public PlayersController(PlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<PlayerDto>> GetAllAsync()
        {
            IEnumerable<Player> players = await _playerService.GetAllAsync();

            return _mapper.Map<IEnumerable<Player>, IEnumerable<PlayerDto>>(players);
        }

        [HttpPost]
        public async Task<PlayerDto> PostAsync(PlayerCreateOrEditDto player)
        {
            _ = player ?? throw new ArgumentNullException(nameof(player));
            Player createdPlayer = await _playerService.CreateAsync(player.Name);
            return _mapper.Map<Player, PlayerDto>(createdPlayer);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<PlayerDto>> UpdateAsync(int id, PlayerCreateOrEditDto player)
        {
            _ = player ?? throw new ArgumentNullException(nameof(player));
            Player? updatedPlayer = await _playerService.UpdateAsync(id, player.Name);
            if (updatedPlayer == null)
            {
                return NotFound();
            }

            return _mapper.Map<Player, PlayerDto>(updatedPlayer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _playerService.DeleteAsync(id);
            return NoContent();
        }
    }
}
