using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class PlayersController : ControllerBase
{
    private readonly PlayerService _playerService;

    [HttpGet]
    public async Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        return await _playerService.GetAllAsync();
    }

    [HttpGet("statistics")]
    public async Task<IEnumerable<PlayerStatisticDto>> GetAllStatisticsAsync()
    {
        return await _playerService.GetAllStatisticsAsync(Request.Headers[Constants.MapStatusHeaderName] == "True");
    }

    [HttpGet("{id}/full")]
    public async Task<ActionResult<PlayerFullDto>> GetFullAsync(string id)
    {
        PlayerFullDto? player = await _playerService.GetFullAsync(id, Request.Headers[Constants.MapStatusHeaderName] == "True");
        if (player is null)
        {
            return NotFound();
        }

        return player;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(string id, PlayerEditDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));
        try
        {
            PlayerDto? updatedPlayer = await _playerService.UpdateAsync(id, dto);
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

    [Authorize(Roles = Roles.Admin)]
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
