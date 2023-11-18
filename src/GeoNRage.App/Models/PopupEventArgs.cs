namespace GeoNRage.App.Models;

[AutoConstructor]
public partial class PopupEventArgs : EventArgs
{
    public bool IsOkCancel { get; }

    [field: AutoConstructorInject]
    public bool ShowProgressBar { get; set; }

    public string Title { get; }

    public string Message { get; }

    public Action? OnOnClick { get; }
}
