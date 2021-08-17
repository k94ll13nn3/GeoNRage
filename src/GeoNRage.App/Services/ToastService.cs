namespace GeoNRage.App.Services;

public class ToastService
{
    public event EventHandler<string>? OnToastRequested;

    public void DisplayToast(string message)
    {
        OnToastRequested?.Invoke(this, message);
    }
}
