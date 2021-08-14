using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Auth;

public static class Roles
{
    public const string Member = nameof(Member);

    public const string Admin = nameof(Admin);

    public const string SuperAdmin = nameof(SuperAdmin);
}

public record UserDto(bool IsAuthenticated, string UserName, Dictionary<string, IEnumerable<string>> Claims);

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

public class UserEditDto
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Passwords do not match!")]
    public string? PasswordConfirm { get; set; }

    public string? PlayerId { get; set; }
}

public class LoginDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
