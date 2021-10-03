using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace GeoNRage.App.Pages;

public partial class LoginPage
{
    [Inject]
    public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    public LoginDto LoginRequest { get; set; } = new LoginDto { RememberMe = true };

    public bool ShowError { get; set; }

    public string Error { get; set; } = string.Empty;

    public bool Loaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if ((await AuthenticationState).User.Identity?.IsAuthenticated == true)
        {
            NavigationManager.NavigateTo("/");
        }
        else
        {
            Loaded = true;
        }
    }

    private async Task OnSubmitAsync()
    {
        ShowError = false;
        HttpResponseMessage response = await GeoNRageStateProvider.LoginAsync(LoginRequest);
        if (!response.IsSuccessStatusCode)
        {
            ShowError = true;
            Error = response.StatusCode == HttpStatusCode.BadRequest
                ? await response.Content.ReadAsStringAsync()
                : "Erreur imprévue";
        }
        else
        {
            NavigationManager.NavigateTo("/");
        }
    }
}
