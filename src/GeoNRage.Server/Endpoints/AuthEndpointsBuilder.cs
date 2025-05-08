using System.Security.Claims;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Models;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Endpoints;

internal static class AuthEndpointsBuilder
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("api/auth").WithTags("Auth");

        group.MapGet("/user", CurrentUserInfo);

        group.MapPost("/login", LoginAsync);
        group.MapPost("/logout", LogoutAsync).RequireAuthorization();
        group.MapPost("/register", RegisterAsync).RequireRole(Roles.SuperAdmin);
        group.MapPost("/edit", EditAsync).RequireRole(Roles.Member);
        group.MapPost("/edit/admin", EditByAdminAsync).RequireRole(Roles.SuperAdmin);

        group.MapDelete("/{userName}", DeleteUserAsync).RequireRole(Roles.SuperAdmin);

        return group;
    }

    private static async Task<Results<Ok, ProblemHttpResult>> LoginAsync(LoginDto request, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        ArgumentNullException.ThrowIfNull(request);

        User? user = await userManager.FindByNameAsync(request.UserName);
        if (user is null)
        {
            return CustomTypedResults.Problem("Utilisateur ou mot de passe invalide.");
        }

        SignInResult singInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!singInResult.Succeeded)
        {
            return CustomTypedResults.Problem("Utilisateur ou mot de passe invalide.");
        }

        await signInManager.SignInAsync(user, request.RememberMe);

        return TypedResults.Ok();
    }

    private static async Task<NoContent> LogoutAsync(SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok, ProblemHttpResult>> RegisterAsync(RegisterDto parameters, UserManager<User> userManager)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var user = new User
        {
            UserName = parameters.UserName
        };

        IdentityResult result = await userManager.CreateAsync(user, parameters.Password);
        if (!result.Succeeded)
        {
            return CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
        }

        result = await userManager.AddToRolesAsync(user, [Roles.Member]);
        return result.Succeeded
            ? TypedResults.Ok()
            : CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
    }

    private static async Task<Results<Ok, ProblemHttpResult>> EditAsync(UserEditDto parameters, UserManager<User> userManager, SignInManager<User> signInManager, ClaimsPrincipal userPrincipal)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        User? user = await userManager.FindByNameAsync(userPrincipal.Identity?.Name!);
        if (user is null)
        {
            return CustomTypedResults.Problem("Utilisateur invalide.");
        }

        IdentityResult result;
        if (parameters.Password is not null)
        {
            string token = await userManager.GeneratePasswordResetTokenAsync(user);
            result = await userManager.ResetPasswordAsync(user, token, parameters.Password);
            if (!result.Succeeded)
            {
                return CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
            }
        }

        user.UserName = parameters.UserName;

        result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
        }

        await signInManager.SignInAsync(user, true);
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, ProblemHttpResult>> EditByAdminAsync(UserEditAdminDto parameters, UserManager<User> userManager, PlayerService playerService)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        User? user = await userManager.FindByNameAsync(parameters.UserName);
        if (user is null)
        {
            return CustomTypedResults.Problem("Utilisateur invalide.");
        }

        if (parameters.PlayerId is not null && await playerService.GetAsync(parameters.PlayerId) is null)
        {
            return CustomTypedResults.Problem("L'id de joueur est invalide.");
        }

        IdentityResult result;

        user.PlayerId = parameters.PlayerId;

        result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
        }

        result = await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
        if (!result.Succeeded)
        {
            return CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
        }

        result = await userManager.AddToRolesAsync(user, parameters.Roles);
        return result.Succeeded
            ? TypedResults.Ok()
            : CustomTypedResults.Problem(result.Errors.FirstOrDefault()?.Description ?? string.Empty);
    }

    private static Ok<UserDto> CurrentUserInfo(ClaimsPrincipal userPrincipal, HttpContext httpContext, PlayerService playerService)
    {
        return TypedResults.Ok(new UserDto
        (
            userPrincipal.Identity?.IsAuthenticated ?? false,
            userPrincipal.Claims.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Select(c => c.Value)),
            playerService.CountChallengesNotDone(userPrincipal.FindFirstValue("PlayerId")!, httpContext.Request.Headers[Constants.MapStatusHeaderName] == "True") > 0
        ));
    }

    private static async Task<NoContent> DeleteUserAsync(string userName, UserManager<User> userManager)
    {
        User? user = await userManager.FindByNameAsync(userName);
        if (user is not null)
        {
            await userManager.DeleteAsync(user);
        }

        return TypedResults.NoContent();
    }
}
