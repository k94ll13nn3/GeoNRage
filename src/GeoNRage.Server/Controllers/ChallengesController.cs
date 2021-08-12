using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class ChallengesController : ControllerBase
{
    private readonly ChallengeService _challengeService;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<ChallengeDto>> GetAllAsync(bool onlyWithoutGame = false, [FromQuery] string[]? playersToExclude = null)
    {
        return await _challengeService.GetAllAsync(onlyWithoutGame, Request.Headers["show-all-maps"] != "True", playersToExclude);
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpGet("admin-view")]
    public async Task<IEnumerable<ChallengeAdminViewDto>> GetAllAsAdminViewAsync()
    {
        return await _challengeService.GetAllAsAdminViewAsync();
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ChallengeDetailDto>> GetAsync(int id)
    {
        ChallengeDetailDto? challenge = await _challengeService.GetAsync(id);
        return challenge ?? (ActionResult<ChallengeDetailDto>)NotFound();
    }

    [AllowAnonymous]
    [HttpPost("import")]
    public async Task<ActionResult<int>> ImportChallengeAsync(ChallengeImportDto dto)
    {
        _ = dto ?? throw new ArgumentNullException(nameof(dto));

        try
        {
            return await _challengeService.ImportChallengeAsync(dto);
        }
        catch (HttpRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _challengeService.DeleteAsync(id);
        return NoContent();
    }
}
