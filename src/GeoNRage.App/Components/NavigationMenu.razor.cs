using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GeoNRage.App.Components
{
    public partial class NavigationMenu
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public IJSRuntime JsRuntime { get; set; } = null!;

        private async Task ChangeThemeAsync(ChangeEventArgs e)
        {
            IJSObjectReference module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/utils.js");
            await module.InvokeVoidAsync("changeTheme", e.Value);
        }
    }
}
