using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.Data;
using GeoNRage.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GeoNRage.Server.Hubs
{
    [Authorize(AuthenticationSchemes = GeoNRageAuthenticationHandler.GeoNRageAuthenticationScheme)]
    public class AppHub : Hub
    {
        private readonly GameService _gameService;

        public AppHub(GameService gameService)
        {
            _gameService = gameService;
        }

        [HubMethodName("JoinGroup")]
        public Task JoinGroupAsync(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"group-${id}");
        }

        [HubMethodName("LoadGames")]
        public async Task LoadGamesAsync()
        {
            IEnumerable<GameBase> games = await _gameService.GetAllBaseAsync();

            await Clients.Caller.SendAsync("ReceiveGames", games);
        }

        [HubMethodName("LoadGame")]
        public async Task LoadGameAsync(int id)
        {
            Game? game = await _gameService.GetByIdAsync(id);
            if (game is not null)
            {
                await Clients.Caller.SendAsync("ReceiveGame", game);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveGame", null);
            }
        }

        [HubMethodName("UpdateValue")]
        public async Task UpdateValueAsync(int id, string key, int score)
        {
            Game? game = await _gameService.GetByIdAsync(id);

            if (game is not null)
            {
                await _gameService.UpdateValueAsync(id, key, score);

                await Clients.OthersInGroup($"group-${id}").SendAsync("ReceiveValue", key, score);
            }
        }
    }
}
