using System.Security.Cryptography;
using GeoNRage.Server.Services;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway;

namespace GeoNRage.Server.Bot;

[AutoConstructor]
public partial class PresenceService : BackgroundService
{
    private readonly DiscordGatewayClient _gatewayClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<(ActivityType type, string name)> _activities = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        MapService mapService = scope.ServiceProvider.GetRequiredService<MapService>();

        _activities.Add((ActivityType.Game, "GeoGuessr"));
        foreach (MapDto map in (await mapService.GetAllAsync()).Where(m => m.IsMapForGame))
        {
            _activities.Add((ActivityType.Game, map.Name));
        }

        // Wait a little to be sure that the bot is started.
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        UpdatePresence();

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(20));
        while (!stoppingToken.IsCancellationRequested)
        {
            await timer.WaitForNextTickAsync(stoppingToken);
            if (!stoppingToken.IsCancellationRequested)
            {
                UpdatePresence();
            }
        }
    }

    private void UpdatePresence()
    {
        (ActivityType type, string name) = _activities[RandomNumberGenerator.GetInt32(_activities.Count)];
        _gatewayClient.SubmitCommand(new UpdatePresence(
            UserStatus.Online,
            false,
            null,
            [new Activity(name, type)]));
    }
}
