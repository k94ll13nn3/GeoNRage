using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
    [AutoConstructor]
    public partial class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationOptions>(_configuration.GetSection(nameof(ApplicationOptions)));

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSignalR();
            services.AddControllers();
            services.AddRazorPages();
            services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));

            string connectionString = _configuration.GetConnectionString("GeoNRageConnection");
            services.AddDbContextPool<GeoNRageDbContext>(options => options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<GeoNRageDbContext>();

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
            services.AddTransient<ChallengeService>();
            services.AddTransient<LocationService>();
            services.AddTransient<GeoGuessrService>();

            var cookieContainer = new CookieContainer();
            services.AddSingleton(cookieContainer);

            services.AddHttpClient("geoguessr", c => c.BaseAddress = new Uri("https://www.geoguessr.com/api/v3/"))
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer
                    };
                });

            services.AddHttpClient("google", c => c.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/"));
            services.AddAutoMapper(typeof(Startup));

            if (_env.IsDevelopment())
            {
                services.AddSwaggerGen();
            }
        }

        public void Configure(IApplicationBuilder app, GeoNRageDbContext context)
        {
            _ = context ?? throw new System.ArgumentNullException(nameof(context));

            context.Database.Migrate();

            app.UseResponseCompression();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Geo N'Rage API"));
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

            app.UseMiddleware<GameMetadataMiddleware>();

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
