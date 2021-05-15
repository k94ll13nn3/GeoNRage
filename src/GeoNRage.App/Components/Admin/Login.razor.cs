using System.Threading.Tasks;
using GeoNRage.App.Authentication;
using GeoNRage.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Admin
{
    public partial class Login
    {
        [Inject]
        public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

        public LoginDto LoginRequest { get; set; } = new LoginDto();

        private async Task OnSubmitAsync()
        {
            await GeoNRageStateProvider.LoginAsync(LoginRequest);
        }
    }
}
