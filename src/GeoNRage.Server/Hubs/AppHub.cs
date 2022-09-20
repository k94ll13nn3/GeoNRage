using System.Security.Claims;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace GeoNRage.Server.Hubs;

[AutoConstructor]
public partial class AppHub : Hub<IAppHub>
{
    private readonly GameService _gameService;

    [HubMethodName("JoinGroup")]
    public async Task JoinGroupAsync(int gameId)
    {
        if (!await UserInGameAsync(gameId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"group-${gameId}");
        if (GetPlayerIdForCurrentUser() is string playerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{playerId}");
        }
    }

    [HubMethodName("NotifyNewPlayer")]
    public async Task NotifyNewPlayerAsync(int gameId)
    {
        if (!await UserInGameAsync(gameId))
        {
            return;
        }

        await Clients.OthersInGroup($"group-${gameId}").NewPlayerAdded();
    }

    [HubMethodName("TauntPlayer")]
    public async Task TauntPlayerAsync(int gameId, string playerId, string imageId)
    {
        if (!await UserInGameAsync(gameId))
        {
            return;
        }

        await Clients.Group($"user_{playerId}").Taunted(imageId, Context.User?.Identity?.Name);
    }

    [HubMethodName("UpdateValue")]
    public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int score)
    {
        if (!await UserInGameAsync(gameId))
        {
            return;
        }

        if (_gameService.Exists(gameId))
        {
            await _gameService.UpdateValueAsync(gameId, challengeId, playerId, round, score);

            await Clients.OthersInGroup($"group-${gameId}").ReceiveValue(challengeId, playerId, round, score);
        }
    }

    private string? GetPlayerIdForCurrentUser()
    {
        return Context.User?.FindFirstValue("PlayerId");
    }

    private async Task<bool> UserInGameAsync(int gameId)
    {
        if (Context.User?.IsInRole(Roles.Member) == true)
        {
            GameDetailDto? game = await _gameService.GetAsync(gameId);
            string? playerId = GetPlayerIdForCurrentUser();
            if (game?.Players.Any(p => p.Id == playerId) == true)
            {
                return true;
            }
        }

        return false;
    }
}
