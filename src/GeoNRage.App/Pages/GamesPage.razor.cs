using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class GamesPage
    {
        private ICollection<GameLightDto> _games = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        public ICollection<GameLightDto> Games { get; } = new List<GameLightDto>();

        public int PageCount { get; set; }

        public int PageSize { get; set; } = 10;

        public int CurrentPage { get; set; } = 1;

        protected override async Task OnInitializedAsync()
        {
            _games = await GamesApi.GetAllLightAsync();
            PageCount = (int)Math.Ceiling(1.0 * _games.Count / PageSize);
            Games.Clear();
            foreach (GameLightDto game in _games.Take(PageSize))
            {
                Games.Add(game);
            }

            StateHasChanged();
        }

        private void ChangePage(int newPage)
        {
            if (newPage >= 1 && newPage <= PageCount)
            {
                CurrentPage = newPage;

                Games.Clear();
                System.Console.WriteLine((CurrentPage - 1) * PageSize);
                foreach (GameLightDto game in _games.Take(CurrentPage * PageSize).Skip((CurrentPage - 1) * PageSize))
                {
                    Games.Add(game);
                }

                StateHasChanged();
            }
        }
    }
}
