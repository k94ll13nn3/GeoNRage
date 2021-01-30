using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Models;
using Microsoft.AspNetCore.SignalR;

namespace GeoNRage.Server.Hubs
{
    public class AppHub : Hub
    {
        private static IEnumerable<Game> Games;

        static AppHub()
        {
            string[] maps = new[] { "France", "Europe", "Monde" };
            string[] columns = new[] { "Kévin", "Jean", "Claude" };
            string[] rows = new[] { "Round 1", "Round 2", "Round 3", "Round 4", "Round 5" };
            var values = new Dictionary<string, int>();

            foreach (string map in maps)
            {
                foreach (string column in columns)
                {
                    foreach (string row in rows)
                    {
                        values[$"{map}_{column}_{row}"] = 0;
                    }
                }
            }

            Game game1 = new Game(20210130, "Dude n'est toujours pas là", columns, rows, maps, values.ToDictionary(k=>k.Key,v=>v.Value));
            game1.Values[$"{maps[0]}_{columns[0]}_{rows[0]}"] = 5000;
            Game game2 = new Game(20210131, "Parties avec les sacs", columns, rows, maps, values.ToDictionary(k => k.Key, v => v.Value));
            Games = new[] { game1, game2 };
        }

        [HubMethodName("JoinGroup")]
        public Task JoinGroupAsync(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"group-${id}");
        }

        [HubMethodName("LoadGames")]
        public async Task LoadGamesAsync()
        {
            await Clients.Caller.SendAsync("ReceiveGames", Games.Cast<GameBase>());
        }

        [HubMethodName("LoadGame")]
        public async Task LoadGameAsync(int id)
        {
            Game? game = Games.FirstOrDefault(g => g.Id == id);
            await Clients.Caller.SendAsync("ReceiveGame", game);
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessageAsync(int id, string columnName, int newValue)
        {
            Games.First(g => g.Id == id).Values[columnName] = newValue;

            await Clients.Group($"group-${id}")/*.AllExcept(new[] { Context.ConnectionId })*/.SendAsync("ReceiveRow", columnName, newValue);
        }
    }
}
