using System.ComponentModel.DataAnnotations;

namespace GeoNRage.Shared.Dtos.Auth;

public static class Roles
{
    public const string Member = nameof(Member);

    public const string Admin = nameof(Admin);

    public const string SuperAdmin = nameof(SuperAdmin);

    /// <summary>
    /// Reserved for disabling features.
    /// </summary>
    public const string None = nameof(None);

    public static IEnumerable<string> All => new[] { Admin, SuperAdmin, Member };
}

public record UserDto(bool IsAuthenticated, string UserName, Dictionary<string, IEnumerable<string>> Claims, string? PlayerId);

public class RegisterDto
{
    [Required(ErrorMessage = "Le champ 'Nom d'utilisateur' est requis.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le champ 'Mot de passe' est requis.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le champ 'Confirmation mot de passe' est requis.")]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passes ne correspondent pas.")]
    public string PasswordConfirm { get; set; } = string.Empty;
}

public class UserEditDto
{
    [Required(ErrorMessage = "Le champ 'Nom d'utilisateur' est requis.")]
    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Les mots de passes ne correspondent pas.")]
    public string? PasswordConfirm { get; set; }
}

public class UserEditAdminDto
{
    [Required(ErrorMessage = "Le champ 'Nom d'utilisateur' est requis.")]
    public string UserName { get; set; } = string.Empty;

    public string? PlayerId { get; set; }

    [Required(ErrorMessage = "Le champ 'Roles' est requis.")]
    public ICollection<string> Roles { get; set; } = new HashSet<string>();
}

public class LoginDto
{
    [Required(ErrorMessage = "Le champ 'Nom d'utilisateur' est requis.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le champ 'Mot de passe' est requis.")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}
