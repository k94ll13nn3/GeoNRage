using GeoNRage.App.Apis;
using GeoNRage.App.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace GeoNRage.App.Pages
{
    public partial class ProfilePage
    {
        [Inject]
        public IAuthApi AuthApi { get; set; } = null!;

        [Inject]
        public GeoNRageStateProvider GeoNRageStateProvider { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public UserEditDto UserEdit { get; set; } = new();

        public EditForm Form { get; set; } = null!;

        public string? Error { get; set; }

        public UserDto User { get; set; } = null!;

        public async Task UpdateUserAsync(EditContext editContext)
        {
            try
            {
                Error = null;
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
                Error = $"Error: {e.Content}";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            User = await AuthApi.CurrentUserInfo();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }
    }
}
