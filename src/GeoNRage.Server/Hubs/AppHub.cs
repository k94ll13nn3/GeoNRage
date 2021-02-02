using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GeoNRage.Server.Hubs
{
    public class AppHub : Hub
    {
        private GeoNRageDbContext _context;

        public AppHub(GeoNRageDbContext context)
        {
            _context = context;
        }

        [HubMethodName("JoinGroup")]
        public Task JoinGroupAsync(int id)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"group-${id}");
        }

        [HubMethodName("LoadGames")]
        public async Task LoadGamesAsync()
        {
            IEnumerable<GameBase> games = (await _context.Games.ToListAsync()).Cast<GameBase>();

            await Clients.Caller.SendAsync("ReceiveGames", games);
        }

        [HubMethodName("LoadGame")]
        public async Task LoadGameAsync(int id)
        {
            Game? game = await _context.Games.Include(g => g.Values).FirstOrDefaultAsync(g => g.Id == id);
            if (game is not null)
            {
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
            Game? game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game is not null)
            {
                Value? value = await _context.Values.FirstOrDefaultAsync(x => x.GameId == id && x.Key == columnName);
                if (value is null)
                {
                    value = new Value
                    {
                        GameId = id,
                        Key = columnName,
                        Score = newValue,
                    };
                    _context.Values.Add(value);
                }
                else
                {
                    value.Score = newValue;
                    _context.Values.Update(value);
                }

                await _context.SaveChangesAsync();

                await Clients.Group($"group-${id}").SendAsync("ReceiveRow", columnName, newValue);
            }
        }
    }
}
