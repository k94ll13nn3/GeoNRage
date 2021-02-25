using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos
{
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
}
