using GeoNRage.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Tasks;

[AutoConstructor]
internal sealed partial class SuperAdminCreationTask : IStartupTask
{
    private readonly IServiceProvider _serviceProvider;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        ApplicationOptions options = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
        User? user = await userManager.FindByNameAsync(options.SuperAdminUserName);
        if (user is null)
        {
            user = new User
            {
                UserName = options.SuperAdminUserName
            };

            await userManager.CreateAsync(user, options.SuperAdminPassword);
            await userManager.AddToRolesAsync(user, Roles.All);
        }
    }
}
