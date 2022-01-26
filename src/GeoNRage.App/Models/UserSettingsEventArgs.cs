namespace GeoNRage.App.Models;

public class UserSettingsEventArgs : EventArgs
{
    public UserSettingsEventArgs(bool allMaps, Theme theme, string changedKey)
    {
        AllMaps = allMaps;
        Theme = theme;
        ChangedKey = changedKey;
    }

    public bool AllMaps { get; }

    public Theme Theme { get; }

    public string ChangedKey { get; set; }
}
