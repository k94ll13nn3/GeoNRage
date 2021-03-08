using System;
using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Http;

namespace GeoNRage.Server
{
    public class GameMetadataMiddleware
    {
        private readonly RequestDelegate _next;

        public GameMetadataMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, GameService gameService)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = gameService ?? throw new ArgumentNullException(nameof(gameService));

            if (context.Request.Headers["User-Agent"] == "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)")
            {
                if (int.TryParse(context.Request.RouteValues["id"] as string, out int id))
                {
                    Game? game = await gameService.GetAsync(id);
                    if (game != null)
                    {
                        await context.Response.WriteAsync($@"<!DOCTYPE html>
<html>
<head>
    <title>Geo'N Rage - {game.Name}</title>
    <meta property=""og:title"" content=""Geo'N Rage - {game.Name}"">
    <meta property=""og:description"" content=""Geo'N Rage - partie du {game.Date.ToShortDateString()}"">
    <meta property=""og:site_name"" content=""Geo'N Rage"">
    <meta name=""twitter:card"" content=""img/site.png"">
    <meta name=""theme-color"" content=""#950740"">
</head>
<body>
</body>
</html>
");
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
