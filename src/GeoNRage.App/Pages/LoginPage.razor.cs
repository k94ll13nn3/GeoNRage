using System.Net;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages;

public partial class LoginPage
{
    [Inject]
    public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

    public LoginDto LoginRequest { get; set; } = new LoginDto { RememberMe = true };

    public bool ShowError { get; set; }

    public string Error { get; set; } = string.Empty;

    public bool Loaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if ((await AuthApi.CurrentUserInfo()).IsAuthenticated)
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
