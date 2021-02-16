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
        public async Task<IEnumerable<GameLightDto>> GetAllAsync()
        {
            IEnumerable<Game> games = await _gameService.GetAllAsync();
            return _mapper.Map<IEnumerable<Game>, IEnumerable<GameLightDto>>(games);
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
    }
}
