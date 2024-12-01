using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Tasks;

[AutoConstructor]
internal sealed partial class RoleCreationTask : IStartupTask
{
    private readonly IServiceProvider _serviceProvider;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (string role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
