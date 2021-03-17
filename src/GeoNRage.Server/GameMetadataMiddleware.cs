using System;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http;

namespace GeoNRage.Server
{
    [AutoConstructor]
    public partial class GameMetadataMiddleware
    {
        private readonly RequestDelegate _next;

        public async Task InvokeAsync(HttpContext context, GameService gameService)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = gameService ?? throw new ArgumentNullException(nameof(gameService));

            bool found = false;
            if (context.Request.Headers["User-Agent"] == "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)"
                && context.Request.Path.StartsWithSegments("/games", StringComparison.OrdinalIgnoreCase))
            {
                string[]? segments = context.Request.Path.Value?.Split('/');
                if (segments?.Length >= 3 && int.TryParse(segments[2], out int id))
                {
                    Game? game = await gameService.GetAsync(id);
                    if (game != null)
                    {
                        found = true;
                        await context.Response.WriteAsync($@"<!DOCTYPE html>
<html>
<head>
    <title>Geo'N Rage - {game.Name}</title>
    <meta property=""og:title"" content=""{game.Name}"">
    <meta property=""og:description"" content=""C'est la France de Jean Castex !"">
    <meta property=""og:site_name"" content=""Geo'N Rage"">
    <meta property=""og:image"" content=""{context.Request.Scheme}://{context.Request.Host}/img/site.png"">
    <meta name=""twitter:card"" content=""summary_large_image"">
    <meta name=""theme-color"" content=""#950740"">
</head>
<body>
</body>
</html>
");
                    }
                }
            }

            if (!found)
            {
                await _next(context);
            }
        }
    }
}
