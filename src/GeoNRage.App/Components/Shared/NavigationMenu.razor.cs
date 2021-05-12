using System.Security.Principal;
using System.Threading.Tasks;
using GeoNRage.App.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

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

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            IIdentity? identity = (await AuthenticationState).User.Identity;
            if (identity is not null)
            {
                Name = identity.Name ?? string.Empty;
            }
        }
        protected override async Task OnInitializedAsync()
        {
            string? theme = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
            IJSObjectReference module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/utils.js");
            await module.InvokeVoidAsync("changeTheme", theme ?? "basic");
        }

        private async Task LogoutClickAsync()
        {
            await GeoNRageStateProvider.LogoutAsync();
        }

        private async Task ChangeThemeAsync(ChangeEventArgs e)
        {
            IJSObjectReference module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/utils.js");
            await module.InvokeVoidAsync("changeTheme", e.Value);
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", e.Value ?? "basic");
        }
    }
}
