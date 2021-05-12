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
    public partial class GamesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly GameService _gameService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<GameDto>> GetAllAsync()
        {
            IEnumerable<Game> games = await _gameService.GetAllAsync(true);
            return _mapper.Map<IEnumerable<Game>, IEnumerable<GameDto>>(games);
        }

        [AllowAnonymous]
        [HttpGet("light")]
        public async Task<IEnumerable<GameLightDto>> GetAllLightAsync()
        {
            IEnumerable<Game> games = await _gameService.GetAllAsync(false);
            return _mapper.Map<IEnumerable<Game>, IEnumerable<GameLightDto>>(games);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetAsync(int id)
        {
            Game? game = await _gameService.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            return _mapper.Map<Game, GameDto>(game);
        }

        [HttpPost]
        public async Task<GameDto> PostAsync(GameCreateOrEditDto game)
        {
            _ = game ?? throw new ArgumentNullException(nameof(game));
            Game createdGame = await _gameService.CreateAsync(game.Name, game.Date, game.MapIds, game.PlayerIds);
            return _mapper.Map<Game, GameDto>(createdGame);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<GameDto>> UpdateAsync(int id, GameCreateOrEditDto game)
        {
            _ = game ?? throw new ArgumentNullException(nameof(game));
            Game? updatedGame = await _gameService.UpdateAsync(id, game.Name, game.Date, game.MapIds, game.PlayerIds);
            if (updatedGame == null)
            {
                return NotFound();
            }

            return _mapper.Map<Game, GameDto>(updatedGame);
        }

        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockAsync(int id)
        {
            await _gameService.LockAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockAsync(int id)
        {
            await _gameService.UnlockAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/reset")]
        public async Task<IActionResult> ResetAsync(int id)
        {
            await _gameService.ResetAsync(id);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("{id}/addPlayer/{playerId}")]
        public async Task<IActionResult> AddPlayerAsync(int id, int playerId)
        {
            await _gameService.AddPlayerAsync(id, playerId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }
    }
}
