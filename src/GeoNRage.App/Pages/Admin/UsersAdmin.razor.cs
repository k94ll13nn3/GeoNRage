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
    public ModalService ModalService { get; set; } = null!;

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
        UserEdit = new UserEditAdminDto { UserName = userName, PlayerId = user.PlayerId, Roles = [.. user.Roles] };
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
            Error = $"Error: {(await e.GetContentAsAsync<ProblemDetails>())?.Detail}";
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
            Error = $"Error: {(await e.GetContentAsAsync<ProblemDetails>())?.Detail}";
        }
    }

    public void ShowUserCreation()
    {
        Error = null;
        ShowCreateForm = true;
        UserRegister = new RegisterDto();
    }

    public async Task DeleteUserAsync(string userName)
    {
        ModalResult result = await ModalService.DisplayOkCancelPopupAsync("Suppression", $"Valider la suppression du joueur {userName} ?");
        if (!result.Cancelled)
        {
            try
            {
                await AuthApi.DeleteUserAsync(userName);
                Users = await AdminApi.GetAllUsersAsAdminViewAsync();
                StateHasChanged();
            }
            catch (ApiException e)
            {
                await ToastService.DisplayErrorToastAsync(e, "player-delete");
            }
        }
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
}
