﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GeoNRage.Server.Tasks
{
    [AutoConstructor]
    public partial class SuperAdminCreationTask : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            ApplicationOptions options = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
            IdentityUser user = await userManager.FindByNameAsync(options.SuperAdminUserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = options.SuperAdminUserName
                };

                await userManager.CreateAsync(user, options.SuperAdminPassword);
                await userManager.AddToRolesAsync(user, new[] { Roles.Admin, Roles.SuperAdmin, Roles.Member });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}