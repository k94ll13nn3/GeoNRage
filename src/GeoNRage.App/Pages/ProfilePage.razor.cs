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

        public UserEditDto User { get; set; } = new();

        public EditForm Form { get; set; } = null!;

        public string? Error { get; set; }

        public async Task UpdateUserAsync(EditContext editContext)
        {
            try
            {
                Error = null;
                if (string.IsNullOrWhiteSpace(User.Password))
                {
                    User.Password = null;
                }

                if (string.IsNullOrWhiteSpace(User.PasswordConfirm))
                {
                    User.PasswordConfirm = null;
                }

                bool formIsValid = editContext.Validate();
                if (formIsValid)
                {
                    await AuthApi.EditAsync(User);
                    GeoNRageStateProvider.NotifyUpdate();
                }
            }
            catch (ApiException e)
            {
                Error = $"Error: {e.Content}";
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            Form?.EditContext?.UpdateCssClassProvider();
        }
    }
}
