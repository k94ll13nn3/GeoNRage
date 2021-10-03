using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly PlayerService _playerService;

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto request)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        User user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return BadRequest("Utilisateur ou mot de passe invalide.");
        }

        Microsoft.AspNetCore.Identity.SignInResult singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!singInResult.Succeeded)
        {
            return BadRequest("Utilisateur ou mot de passe invalide.");
        }

        await _signInManager.SignInAsync(user, request.RememberMe);

        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto parameters)
    {
        _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

        var user = new User
        {
            UserName = parameters.UserName
        };

        IdentityResult result = await _userManager.CreateAsync(user, parameters.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        result = await _userManager.AddToRolesAsync(user, new[] { Roles.Member });
        return result.Succeeded
            ? Ok()
            : BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [Authorize(Roles = Roles.Member)]
    [HttpPost("edit")]
    public async Task<IActionResult> EditAsync(UserEditDto parameters)
    {
        _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

        User user = await _userManager.FindByNameAsync(User.Identity?.Name);
        if (user is null)
        {
            return BadRequest("Utilisateur invalide.");
        }

        IdentityResult result;
        if (parameters.Password is not null)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            result = await _userManager.ResetPasswordAsync(user, token, parameters.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }
        }

        user.UserName = parameters.UserName;

        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        await _signInManager.SignInAsync(user, true);
        return Ok();
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpPost("edit/admin")]
    public async Task<IActionResult> EditByAdminAsync(UserEditAdminDto parameters)
    {
        _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

        User user = await _userManager.FindByNameAsync(parameters.UserName);
        if (user is null)
        {
            return BadRequest("Utilisateur invalide.");
        }

        if (parameters.PlayerId is not null && await _playerService.GetAsync(parameters.PlayerId) is null)
        {
            return BadRequest("L'id de joueur est invalide.");
        }

        IdentityResult result;

        user.PlayerId = parameters.PlayerId;

        result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        result = await _userManager.AddToRolesAsync(user, parameters.Roles);
        return result.Succeeded
            ? Ok()
            : BadRequest(result.Errors.FirstOrDefault()?.Description);
    }

    [HttpGet("user")]
    public async Task<UserDto> CurrentUserInfoAsync()
    {
        User user = await _userManager.GetUserAsync(User);
        return new UserDto
        (
            User.Identity?.IsAuthenticated ?? false,
            User.Identity?.Name ?? string.Empty,
            User.Claims.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Select(c => c.Value)),
            user?.PlayerId
        );
    }

    [Authorize(Roles = Roles.SuperAdmin)]
    [HttpDelete("{userName}")]
    public async Task<IActionResult> DeleteUserAsync(string userName)
    {
        User user = await _userManager.FindByNameAsync(userName);
        if (user is not null)
        {
            await _userManager.DeleteAsync(user);
        }

        return NoContent();
    }
}
