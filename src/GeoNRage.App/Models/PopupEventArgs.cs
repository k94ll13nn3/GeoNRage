namespace GeoNRage.App.Models;

public class PopupEventArgs : EventArgs
{
    public PopupEventArgs(bool isOkCancel, bool showProgressBar, string title, string message, Action? onOnClick)
    {
        IsOkCancel = isOkCancel;
        ShowProgressBar = showProgressBar;
        Title = title;
        Message = message;
        OnOnClick = onOnClick;
    }

    public bool IsOkCancel { get; }

    public bool ShowProgressBar { get; set; }

    public string Title { get; }

    public string Message { get; }

    public Action? OnOnClick { get; }
}
