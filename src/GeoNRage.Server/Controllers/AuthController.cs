using System;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AutoConstructor]
    public partial class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        [AutoConstructorInject("options?.Value", "options", typeof(IOptions<ApplicationOptions>))]
        private readonly ApplicationOptions _options;

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            IdentityUser user = await _userManager.FindByNameAsync(request.UserName);
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto parameters)
        {
            _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

            if (!_options.CanRegister)
            {
                return BadRequest("Registration not enabled.");
            }

            var user = new IdentityUser
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

        [HttpGet("user")]
        public UserDto CurrentUserInfo()
        {
            return new UserDto
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name ?? string.Empty,
                Claims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }
    }
}
