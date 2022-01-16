namespace GeoNRage.App.Models;

public class UserSettingsEventArgs : EventArgs
{
    public UserSettingsEventArgs(bool allMaps, Theme theme)
    {
        AllMaps = allMaps;
        Theme = theme;
    }

    public bool AllMaps { get; }

    public Theme Theme { get; }
}
