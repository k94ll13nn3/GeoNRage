namespace GeoNRage.App.Models;

[AutoConstructor]
public partial class UserSettingsEventArgs : EventArgs
{
    public bool AllMaps { get; }

    public Theme Theme { get; }

    public bool SeasonalStyle { get; }

    [field: AutoConstructorInject]
    public string ChangedKey { get; set; }
}
