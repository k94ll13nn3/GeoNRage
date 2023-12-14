namespace GeoNRage.App.Components;

public partial class Snowflakes
{
    private readonly bool _enabled = DateTime.Now.Month is 12;
    private bool _seasonalStyle;

    protected override async Task OnInitializedAsync()
    {
        _seasonalStyle = (await UserSettingsService.GetAsync()).SeasonalStyle;
    }

    internal override void OnSettingsChanged(object? sender, UserSettingsEventArgs e)
    {
        _seasonalStyle = e.SeasonalStyle;
        StateHasChanged();
    }
}
