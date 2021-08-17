namespace GeoNRage.App.Services;

public record ToastData(string Message, TimeSpan? Duration);

public class ToastService
{
    public event EventHandler<ToastData>? OnToastRequested;

    public void DisplayToast(string message, TimeSpan? duration)
    {
        OnToastRequested?.Invoke(this, new(message, duration));
    }
}
