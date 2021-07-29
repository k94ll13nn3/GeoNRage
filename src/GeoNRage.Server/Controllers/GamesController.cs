using System;
using System.Collections.Generic;
using System.Net.Http;
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
        public async Task<ActionResult<GameDto>> CreateAsync(GameCreateOrEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            try
            {
                Game createdGame = await _gameService.CreateAsync(dto);
                return _mapper.Map<Game, GameDto>(createdGame);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<GameDto>> UpdateAsync(int id, GameCreateOrEditDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));
            try
            {
                Game? updatedGame = await _gameService.UpdateAsync(id, dto);
                if (updatedGame is null)
                {
                    return NotFound();
                }

                return _mapper.Map<Game, GameDto>(updatedGame);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("{id}/add-player/{playerId}")]
        public async Task<ActionResult<GameDto>> AddPlayerAsync(int id, string playerId)
        {
            try
            {
                Game? updatedGame = await _gameService.AddPlayerAsync(id, playerId);
                if (updatedGame is null)
                {
                    return NotFound();
                }

                return _mapper.Map<Game, GameDto>(updatedGame);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _gameService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/import-challenge")]
        public async Task<IActionResult> ImportChallengeAsync(int id, ChallengeImportDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            try
            {
                Game? game = await _gameService.GetAsync(id);
                if (game == null)
                {
                    return NotFound();
                }

                await _gameService.ImportChallengeAsync(id, dto);

                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
