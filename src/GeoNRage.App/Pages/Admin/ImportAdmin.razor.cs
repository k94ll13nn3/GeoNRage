using System.Collections.Generic;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages.Admin
{
    public partial class ImportAdmin
    {
        [Inject]
        public IGamesApi GamesApi { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public IEnumerable<GameDto> Games { get; set; } = null!;

        public int SelectedGameId { get; set; }

        public string Link { get; set; } = string.Empty;

        public ChallengeImportDto ChallengeImportDto { get; set; } = new ChallengeImportDto();

        public ChallengeDto? Result { get; set; }

        public bool DataImported { get; set; }

        public async Task ImportAsync()
        {
            Result = null;
            Result = await GamesApi.ImportChallengeAsync(SelectedGameId, ChallengeImportDto);
            DataImported = ChallengeImportDto.PersistData;
        }

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllAsync();
        }
    }
}
