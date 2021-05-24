using System.Security.Principal;
using System.Threading.Tasks;
using GeoNRage.App.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GeoNRage.App.Components.Shared
{
    public partial class NavigationMenu
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        [Inject]
        public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public bool ShowMenu { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            IIdentity? identity = (await AuthenticationState).User.Identity;
            if (identity is not null)
            {
                Name = identity.Name ?? string.Empty;
            }
        }

        private async Task LogoutClickAsync()
        {
            await GeoNRageStateProvider.LogoutAsync();
        }

        private void ToggleMenu()
        {
            ShowMenu = !ShowMenu;
        }
    }
}
