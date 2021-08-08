using System;
using System.Threading.Tasks;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;

namespace GeoNRage.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Logger logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();
            try
            {
                IHost host = CreateHostBuilder(args).Build();

                using (IServiceScope scope = host.Services.CreateScope())
                {
                    GeoNRageDbContext context = scope.ServiceProvider.GetRequiredService<GeoNRageDbContext>();
                    context.Database.Migrate();

                    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    ApplicationOptions options = (scope.ServiceProvider.GetRequiredService<IOptions<ApplicationOptions>>()).Value;
                    string[]? roles = new[] { Roles.Admin, Roles.SuperAdmin, Roles.Member };
                    foreach (string? role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    IdentityUser user = await userManager.FindByNameAsync(options.SuperAdminUserName);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = options.SuperAdminUserName
                        };

                        await userManager.CreateAsync(user, options.SuperAdminPassword);
                        await userManager.AddToRolesAsync(user, roles);
                    }
                }

                await host.RunAsync();
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
        }
    }
}
