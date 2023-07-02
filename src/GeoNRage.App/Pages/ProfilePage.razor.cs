using System.Security.Claims;
using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages;

public partial class ProfilePage
{
    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    [Inject]
    public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public UserEditDto UserEdit { get; set; } = new();

    public EditForm Form { get; set; } = null!;

    public ClaimsPrincipal User { get; set; } = null!;

    public async Task UpdateUserAsync(EditContext editContext)
    {
        ArgumentNullException.ThrowIfNull(editContext);

        try
        {
            if (string.IsNullOrWhiteSpace(UserEdit.Password))
            {
                UserEdit.Password = null;
            }

            if (string.IsNullOrWhiteSpace(UserEdit.PasswordConfirm))
            {
                UserEdit.PasswordConfirm = null;
            }

            bool formIsValid = editContext.Validate();
            if (formIsValid)
            {
                await AuthApi.EditAsync(UserEdit);
                GeoNRageStateProvider.NotifyUpdate();
            }
        }
        catch (ApiException e)
        {
            await ToastService.DisplayErrorToastAsync(e, "profile-update");
        }
    }

    protected override async Task OnInitializedAsync()
    {
        User = (await AuthenticationState).User;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Form?.EditContext?.UpdateCssClassProvider();
    }
}
