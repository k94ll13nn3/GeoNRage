using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GeoNRage.App.Core;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages
{
    public partial class LoginPage
    {
        [Inject]
        public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public LoginDto LoginRequest { get; set; } = new LoginDto { RememberMe = true };

        public bool ShowError { get; set; }

        public string Error { get; set; } = string.Empty;

        private async Task OnSubmitAsync()
        {
            ShowError = false;
            HttpResponseMessage response = await GeoNRageStateProvider.LoginAsync(LoginRequest);
            if (!response.IsSuccessStatusCode)
            {
                ShowError = true;
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Error = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Error = "Erreur imprévue";
                }
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }
    }
}
