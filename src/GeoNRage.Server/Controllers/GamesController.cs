using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Data;
using GeoNRage.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _gameService;

        public GamesController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _gameService.GetAllGamesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetAsync(int id)
        {
            Game? game = await _gameService.GetGameAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            foreach (Map map in game.Maps)
            {
                map.Games = Array.Empty<Game>();
            }

            foreach (Player player in game.Players)
            {
                player.Games = Array.Empty<Game>();
            }

            foreach (Value value in game.Values)
            {
                value.Player = null!;
                value.Map = null!;
                value.Game = null!;
            }

            return game;
        }
    }
}
