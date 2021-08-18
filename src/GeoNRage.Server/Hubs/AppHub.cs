using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace GeoNRage.Server.Hubs;

[AutoConstructor]
public partial class AppHub : Hub
{
    private readonly GameService _gameService;
    private readonly UserManager<User> _userManager;

    [HubMethodName("JoinGroup")]
    public async Task JoinGroupAsync(int gameId)
    {
        if (!await UserInGame(gameId))
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"group-${gameId}");
    }

    [HubMethodName("NotifyNewPlayer")]
    public async Task NotifyNewPlayerAsync(int gameId)
    {
        if (!await UserInGame(gameId))
        {
            return;
        }

        await Clients.OthersInGroup($"group-${gameId}").SendAsync("NewPlayerAdded");
    }

    [HubMethodName("UpdateValue")]
    public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int score)
    {
        if (!await UserInGame(gameId))
        {
            return;
        }

        GameDetailDto? game = await _gameService.GetAsync(gameId);

        if (game is not null)
        {
            await _gameService.UpdateValueAsync(gameId, challengeId, playerId, round, score);

            await Clients.OthersInGroup($"group-${gameId}").SendAsync("ReceiveValue", challengeId, playerId, round, score);
        }
    }

    private async Task<bool> UserInGame(int gameId)
    {
        if (Context.User?.IsInRole(Roles.Member) == true)
        {
            GameDetailDto? game = await _gameService.GetAsync(gameId);
            User user = await _userManager.GetUserAsync(Context.User);
            if (game?.Players.Any(p => p.Id == user.PlayerId) == true)
            {
                return true;
            }
        }

        return false;
    }
}
