using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Data.Entities;
using GeoNRage.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayersController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _playerService.GetAllAsync();
        }
    }
}
