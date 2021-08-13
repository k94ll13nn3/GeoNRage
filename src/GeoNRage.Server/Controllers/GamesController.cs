using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        return await _gameService.GetAllAsync();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("admin-view")]
    public async Task<IEnumerable<GameAdminViewDto>> GetAllAsAdminViewAsync()
    {
        return await _gameService.GetAllAsAdminViewAsync();
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDetailDto>> GetAsync(int id)
    {
        GameDetailDto? game = await _gameService.GetAsync(id);
        return game ?? (ActionResult<GameDetailDto>)NotFound();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(GameCreateOrEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            int gameId = await _gameService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAsync), new { id = gameId }, null);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, GameCreateOrEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            Game? updatedGame = await _gameService.UpdateAsync(id, dto);
            if (updatedGame is null)
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

    [AllowAnonymous]
    [HttpPost("{id}/add-player")]
    public async Task<IActionResult> AddPlayerAsync(int id, [FromBody] string playerId)
    {
        try
        {
            GameDetailDto? game = await _gameService.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            await _gameService.AddPlayerAsync(id, playerId);

            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _gameService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{id}/update-challenges")]
    public async Task<IActionResult> UpdateChallengesAsync(int id)
    {
        try
        {
            GameDetailDto? game = await _gameService.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            await _gameService.ImportChallengeAsync(id);

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
