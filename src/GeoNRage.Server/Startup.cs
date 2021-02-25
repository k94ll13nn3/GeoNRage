using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Hubs;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GeoNRage.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationOptions>(Configuration.GetSection(nameof(ApplicationOptions)));

            services.AddSignalR();
            services.AddControllers();
            services.AddRazorPages();
            services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));

            string connectionString = Configuration.GetConnectionString("GeoNRageConnection");
            services.AddDbContextPool<GeoNRageDbContext>(options => options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<GeoNRageDbContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.AddTransient<GameService>();
            services.AddTransient<MapService>();
            services.AddTransient<PlayerService>();

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GeoNRageDbContext context)
        {
            _ = context ?? throw new System.ArgumentNullException(nameof(context));

            context.Database.Migrate();

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<AppHub>("/apphub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
