using System;
using System.Security.Principal;
using System.Threading.Tasks;
using GeoNRage.App.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace GeoNRage.App.Components.Shared
{
    public partial class NavigationMenu : IDisposable
    {
        private bool _disposedValue;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        public string Name { get; set; } = string.Empty;

        [Inject]
        public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public bool ShowMenu { get; set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected override async Task OnParametersSetAsync()
        {
            IIdentity? identity = (await AuthenticationState).User.Identity;
            if (identity is not null)
            {
                Name = identity.Name ?? string.Empty;
            }
        }

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += LocationChanged;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NavigationManager.LocationChanged -= LocationChanged;
                }

                _disposedValue = true;
            }
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            ShowMenu = false;
            StateHasChanged();
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
