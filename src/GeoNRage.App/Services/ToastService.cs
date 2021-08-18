namespace GeoNRage.App.Services;

public record ToastData(Guid Id, string Message, TimeSpan? Duration, ToastType ToastType);

public enum ToastType
{
    Primary = 0,
    Information,
    Link,
    Success,
    Warning,
    Error,
}

public class ToastService
{
    public event EventHandler<ToastData>? OnToastRequested;

    public void DisplayToast(string message = "", TimeSpan? duration = null, ToastType toastType = ToastType.Primary)
    {
        OnToastRequested?.Invoke(this, new(Guid.NewGuid(), message, duration, toastType));
    }
}
