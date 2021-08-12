using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;

namespace GeoNRage.Server.Tasks
{
    [AutoConstructor]
    public partial class RoleCreationTask : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (string role in new[] { Roles.Admin, Roles.SuperAdmin, Roles.Member })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
