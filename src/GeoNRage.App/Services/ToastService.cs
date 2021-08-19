using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public record ToastData(string Id, RenderFragment Content, TimeSpan? Duration, ToastType ToastType);

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

    public void DisplayToast(string message, TimeSpan? duration = null, ToastType toastType = ToastType.Primary, string id = "")
    {
        DisplayToast(builder => builder.AddContent(1, message), duration, toastType, id);
    }

    public void DisplayToast(RenderFragment content, TimeSpan? duration = null, ToastType toastType = ToastType.Primary, string id = "")
    {
        OnToastRequested?.Invoke(this, new(string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id, content, duration, toastType));
    }
}
