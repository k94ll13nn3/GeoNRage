using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly GameService _gameService;

        public GamesController(GameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<GameDto>> GetAllAsync()
        {
            IEnumerable<Game> games = await _gameService.GetAllAsync();
            return _mapper.Map<IEnumerable<Game>, IEnumerable<GameDto>>(games);
        }

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
        public async Task<ActionResult<GameDto>> PostAsync(GameCreateOrEditDto game)
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
        public async Task LockAsync(int id)
        {
            await _gameService.LockAsync(id);
        }

        [HttpPost("{id}/reset")]
        public async Task ResetAsync(int id)
        {
            await _gameService.ResetAsync(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }
    }
}
