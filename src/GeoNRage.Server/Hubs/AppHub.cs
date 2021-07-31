using System.Threading.Tasks;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos.Games;
using Microsoft.AspNetCore.SignalR;

namespace GeoNRage.Server.Hubs
{
    [AutoConstructor]
    public partial class AppHub : Hub
    {
        private readonly GameService _gameService;

        [HubMethodName("JoinGroup")]
        public Task JoinGroupAsync(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"group-${id}");
        }

        [HubMethodName("UpdateValue")]
        public async Task UpdateValueAsync(int gameId, int challengeId, string playerId, int round, int score)
        {
            GameDetailDto? game = await _gameService.GetAsync(gameId);

            if (game is not null)
            {
                await _gameService.UpdateValueAsync(gameId, challengeId, playerId, round, score);

                await Clients.OthersInGroup($"group-${gameId}").SendAsync("ReceiveValue", challengeId, playerId, round, score);
            }
        }
    }
}
