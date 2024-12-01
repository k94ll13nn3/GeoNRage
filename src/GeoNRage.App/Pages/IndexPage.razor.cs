using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Pages;

public partial class IndexPage
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo("/games");
    }
}
