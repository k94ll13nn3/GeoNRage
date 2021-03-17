using System.Threading.Tasks;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
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
        public async Task UpdateValueAsync(int id, int mapId, int playerId, int round, int score)
        {
            Game? game = await _gameService.GetAsync(id);

            if (game is not null)
            {
                await _gameService.UpdateValueAsync(id, mapId, playerId, round, score);

                await Clients.OthersInGroup($"group-${id}").SendAsync("ReceiveValue", mapId, playerId, round, score);
            }
        }
    }
}
