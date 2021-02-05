using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeoNRage.Server.Pages
{
    public class AdminModel : PageModel
    {
        private readonly GameService _gameService;

        public AdminModel(GameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty, Required]
        public string GameName { get; set; } = string.Empty;

        [BindProperty, Required]
        public string GameMaps { get; set; } = "France_Europe_Monde";

        [BindProperty, Required]
        public string GamePlayers { get; set; } = string.Empty;

        public IEnumerable<Game> Games { get; set; } = Enumerable.Empty<Game>();

        public async Task OnGetAsync()
        {
            Games = await _gameService.GetAllAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await _gameService.CreateGameAsync(GameName, GameMaps.Split('_'), GamePlayers.Split('_'));

                return RedirectToPage("Admin");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(int id, string name, string maps, string players)
        {
            await _gameService.UpdateGameAsync(id, name, maps.Split('_'), players.Split('_'));
            return RedirectToPage("Admin");
        }

        public async Task<IActionResult> OnPostResetAsync(int id)
        {
            await _gameService.ResetGameAsync(id);
            return RedirectToPage("Admin");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _gameService.DeleteGameAsync(id);
            return RedirectToPage("Admin");
        }

        public async Task<IActionResult> OnPostLockAsync(int id)
        {
            await _gameService.LockGameAsync(id);
            return RedirectToPage("Admin");
        }
    }
}
