using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Auth
{
    public record UserDto(bool IsAuthenticated, string UserName, Dictionary<string, string> Claims);

    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
        public string PasswordConfirm { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
