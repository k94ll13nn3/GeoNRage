namespace GeoNRage.App.Layouts.Main;

public partial class SiteHeader
{
    public Theme Theme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Theme = (await UserSettingsService.GetAsync()).Theme;
    }

    internal override void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        Theme = e.Theme;
        StateHasChanged();
    }
}
