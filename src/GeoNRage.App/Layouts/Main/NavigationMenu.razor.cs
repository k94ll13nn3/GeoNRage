using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace GeoNRage.App.Layouts.Main;

public partial class NavigationMenu
{
    private bool _disposedValue;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    public ClaimsPrincipal User { get; set; } = null!;

    [Inject]
    public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

    public bool ShowMenu { get; set; }

    public bool HasNotifications { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        User = (await AuthenticationState).User;
        HasNotifications = (await AuthApi.CurrentUserInfo()).HasNotifications;
    }

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += LocationChanged;
        base.OnInitialized();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                NavigationManager.LocationChanged -= LocationChanged;
            }

            _disposedValue = true;
        }

        base.Dispose(disposing);
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        ShowMenu = false;
        StateHasChanged();
    }

    private async Task LogoutClickAsync()
    {
        await GeoNRageStateProvider.LogoutAsync();
        NavigationManager.NavigateTo("/");
    }

    private void ToggleMenu()
    {
        ShowMenu = !ShowMenu;
    }

    internal override async void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        HasNotifications = (await AuthApi.CurrentUserInfo()).HasNotifications;
        StateHasChanged();
    }
}
