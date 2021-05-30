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

        public async Task InvokeAsync(HttpContext context, GameService gameService, PlayerService playerService)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _ = playerService ?? throw new ArgumentNullException(nameof(playerService));

            bool found = false;
            string name = string.Empty;
            if (context.Request.Headers["User-Agent"] == "Mozilla/5.0 (compatible; Discordbot/2.0; +https://discordapp.com)")
            {
                if (context.Request.Path.StartsWithSegments("/games", StringComparison.OrdinalIgnoreCase))
                {
                    string[]? segments = context.Request.Path.Value?.Split('/');
                    if (segments?.Length >= 3 && int.TryParse(segments[2], out int id))
                    {
                        Game? game = await gameService.GetAsync(id);
                        if (game != null)
                        {
                            found = true;
                            name = game.Name;
                        }
                    }
                }

                if (context.Request.Path.StartsWithSegments("/players", StringComparison.OrdinalIgnoreCase))
                {
                    string[]? segments = context.Request.Path.Value?.Split('/');
                    if (segments?.Length >= 3)
                    {
                        Player? player = await playerService.GetFullAsync(segments[2]);
                        if (player != null)
                        {
                            found = true;
                            name = player.Name;
                        }
                    }
                }
            }

            if (found && !string.IsNullOrWhiteSpace(name))
            {
                await context.Response.WriteAsync($@"<!DOCTYPE html>
<html>
<head>
    <title>Geo'N Rage - {name}</title>
    <meta property=""og:title"" content=""{name}"">
    <meta property=""og:description"" content=""Venez jouer à GeoGuessr dans la joie, la bonne humeur, et la rage."">
    <meta property=""og:site_name"" content=""Geo'N Rage"">
</head>
<body>
</body>
</html>
");
            }
            else
            {
                await _next(context);
            }
        }
    }
}
