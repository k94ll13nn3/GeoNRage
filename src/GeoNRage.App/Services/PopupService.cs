namespace GeoNRage.App.Services;

public class PopupService
{
    public event EventHandler<PopupEventArgs>? OnPopupUpdated;

    public event EventHandler? OnPopupHidden;

    public void DisplayOkCancelPopup(string title, string message, Action onOkClick)
    {
        var args = new PopupEventArgs(true, false, title, message, onOkClick);
        OnPopupUpdated?.Invoke(this, args);
    }

    public void DisplayLoader(string title)
    {
        var args = new PopupEventArgs(true, true, title, string.Empty, null);
        OnPopupUpdated?.Invoke(this, args);
    }

    public void HidePopup()
    {
        OnPopupHidden?.Invoke(this, new());
    }
}
