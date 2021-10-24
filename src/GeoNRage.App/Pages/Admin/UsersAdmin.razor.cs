using GeoNRage.App.Apis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages.Admin;

public partial class UsersAdmin
{
    [Inject]
    public IAdminApi AdminApi { get; set; } = null!;

    [Inject]
    public IAuthApi AuthApi { get; set; } = null!;

    [Inject]
    public IPlayersApi PlayersApi { get; set; } = null!;

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    public IEnumerable<PlayerDto> Players { get; set; } = null!;

    public IEnumerable<UserAminViewDto> Users { get; set; } = null!;

    public bool ShowCreateForm { get; set; }

    public bool ShowEditForm { get; set; }

    public RegisterDto UserRegister { get; set; } = new();

    public UserEditAdminDto UserEdit { get; set; } = new();

    public EditForm Form { get; set; } = null!;

    public string? Error { get; set; }

    public void EditUser(string userName)
    {
        Error = null;
        ShowEditForm = true;
        UserAminViewDto user = Users.Single(u => u.UserName == userName);
        UserEdit = new UserEditAdminDto { UserName = userName, PlayerId = user.PlayerId, Roles = user.Roles.ToList() };
    }

    public async Task EditUserAsync()
    {
        try
        {
            Error = null;
            if (string.IsNullOrWhiteSpace(UserEdit.PlayerId))
            {
                UserEdit.PlayerId = null;
            }

            await AuthApi.EditByAdminAsync(UserEdit);
            ShowEditForm = false;
            Users = await AdminApi.GetAllUsersAsAdminViewAsync();
            StateHasChanged();
        }
        catch (ApiException e)
        {
            Error = $"Error: {e.Content}";
        }
    }

    public async Task CreateUserAsync()
    {
        try
        {
            Error = null;
            await AuthApi.RegisterAsync(UserRegister);
            ShowCreateForm = false;
            Users = await AdminApi.GetAllUsersAsAdminViewAsync();
            StateHasChanged();
        }
        catch (ApiException e)
        {
            Error = $"Error: {e.Content}";
        }
    }

    public void ShowUserCreation()
    {
        Error = null;
        ShowCreateForm = true;
        UserRegister = new RegisterDto();
    }

    public void DeleteUser(string userName)
    {
        PopupService.DisplayOkCancelPopup("Suppression", $"Valider la suppression du joueur {userName} ?", () => OnConfirmDeleteAsync(userName), false);
    }

    protected override async Task OnInitializedAsync()
    {
        Users = await AdminApi.GetAllUsersAsAdminViewAsync();
        Players = await PlayersApi.GetAllAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        Form?.EditContext?.UpdateCssClassProvider();
    }

    private async void OnConfirmDeleteAsync(string userName)
    {
        try
        {
            await AuthApi.DeleteUserAsync(userName);
            Users = await AdminApi.GetAllUsersAsAdminViewAsync();
            StateHasChanged();
        }
        catch (ApiException e)
        {
            ToastService.DisplayToast(e.Content ?? "Une erreur est survenue", null, ToastType.Error, "player-delete", true);
        }
    }
}
