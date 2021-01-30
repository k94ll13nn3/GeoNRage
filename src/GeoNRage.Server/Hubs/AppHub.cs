using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GeoNRage.Models;
using Microsoft.AspNetCore.SignalR;
using Octokit;

namespace GeoNRage.Server.Hubs
{
    public class AppHub : Hub
    {
        private readonly GitHubClient _client;

        public AppHub()
        {
            _client = new GitHubClient(new ProductHeaderValue("GeoNRage"));
            _client.Credentials = new Credentials("776885616e24d13df071c235025b49050f595159"); //TODO:delete before release
        }

        [HubMethodName("JoinGroup")]
        public Task JoinGroupAsync(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"group-${id}");
        }

        [HubMethodName("LoadGames")]
        public async Task LoadGamesAsync()
        {
            var games = new List<Game>();
            Gist gist = await _client.Gist.Get("55c7c9c88b89b438ae4bd500d0de451e");
            foreach ((_, GistFile file) in gist.Files)
            {
                games.Add(JsonSerializer.Deserialize<Game>(file.Content)!);
            }

            await Clients.Caller.SendAsync("ReceiveGames", games.Cast<GameBase>());
        }

        [HubMethodName("LoadGame")]
        public async Task LoadGameAsync(int id)
        {
            Gist gist = await _client.Gist.Get("55c7c9c88b89b438ae4bd500d0de451e");
            if (gist.Files.ContainsKey($"{id}.json"))
            {
                Game game = JsonSerializer.Deserialize<Game>(gist.Files[$"{id}.json"].Content)!;
                await Clients.Caller.SendAsync("ReceiveGame", game);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveGame", null);
            }
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(int id, string columnName, int newValue)
        {
            Gist gist = await _client.Gist.Get("55c7c9c88b89b438ae4bd500d0de451e");
            Game game = JsonSerializer.Deserialize<Game>(gist.Files[$"{id}.json"].Content)!;
            game.Values[columnName] = newValue;
            var update = new GistUpdate();
            update.Files[$"{id}.json"] = new GistFileUpdate() { Content = JsonSerializer.Serialize(game) };
            await _client.Gist.Edit("55c7c9c88b89b438ae4bd500d0de451e", update);

            await Clients.Group($"group-${id}").SendAsync("ReceiveRow", columnName, newValue);
        }
    }
}
