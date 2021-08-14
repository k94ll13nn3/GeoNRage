using GeoNRage.Server.Entities;
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginDto request)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        User user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return BadRequest("Invalid password or user.");
        }

        Microsoft.AspNetCore.Identity.SignInResult singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!singInResult.Succeeded)
        {
            return BadRequest("Invalid password or user.");
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

        return await LoginAsync(new LoginDto
        {
            UserName = parameters.UserName,
            Password = parameters.Password
        });
    }

    [Authorize]
    [HttpPost("edit")]
    public async Task<IActionResult> EditAsync(UserEditDto parameters)
    {
        _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

        User user = await _userManager.FindByNameAsync(User.Identity?.Name);
        if (user is null)
        {
            return BadRequest("Invalid user.");
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

    [HttpGet("user")]
    public async Task<UserDto> CurrentUserInfo()
    {
        User user = await _userManager.FindByNameAsync(User.Identity?.Name);
        return new UserDto
        (
            User.Identity?.IsAuthenticated ?? false,
            User.Identity?.Name ?? string.Empty,
            User.Claims.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Select(c => c.Value)),
            user?.PlayerId
        );
    }
}
