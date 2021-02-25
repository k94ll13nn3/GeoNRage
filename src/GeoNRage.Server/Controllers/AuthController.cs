using System;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationOptions _options;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationOptions> options)
        {
            _ = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _ = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            _userManager = userManager;
            _signInManager = signInManager;
            _options = options.Value;
        }

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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto parameters)
        {
            _ = parameters ?? throw new ArgumentNullException(nameof(parameters));

            if (!_options.CanRegister)
            {
                return BadRequest("Registration not enabled.");
            }

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
