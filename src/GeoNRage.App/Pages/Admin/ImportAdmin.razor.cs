using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

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

        public ChallengeImportDto ChallengeImportDto { get; set; } = new ChallengeImportDto();

        public ChallengeDto? Result { get; set; }

        public bool DataImported { get; set; }

        public string? Error { get; set; }

        public EditForm Form { get; set; } = null!;

        public async Task ImportAsync()
        {
            try
            {
                Error = null;
                Result = null;
                Result = await GamesApi.ImportChallengeAsync(SelectedGameId, ChallengeImportDto);
                DataImported = ChallengeImportDto.PersistData;
            }
            catch (ValidationApiException e)
            {
                Error = string.Join(",", e.Content?.Errors.Select(x => string.Join(",", x.Value)) ?? Array.Empty<string>());
            }
            catch (ApiException e)
            {
                Error = e.Content;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Games = await GamesApi.GetAllAsync();
            SelectedGameId = Games.First().Id;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }
    }
}
