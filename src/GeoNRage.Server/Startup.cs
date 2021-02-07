using System.Linq;
using GeoNRage.Data;
using GeoNRage.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GeoNRage.Server
{
    public class Startup
    {
        readonly string CorsOrigins = "F1E62903-2100-4DDA-9339-40F7EF61C9CC";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(CorsOrigins, builder => builder.WithOrigins(Configuration.GetValue<string>("AllowedHosts"))));

            services.AddRazorPages();

            services.AddSignalR();

            services.AddResponseCompression(opts => opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" }));

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<GeoNRageDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.MigrationsAssembly("GeoNRage.Server")));

            services.AddTransient<GameService>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            if (!env.IsDevelopment())
            {
                app.UseCors(CorsOrigins);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<AppHub>("/apphub");
            });
        }
    }
}
