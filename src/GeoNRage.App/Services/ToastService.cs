using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public class ToastService
{
    public event EventHandler<ToastData>? OnToastRequested;

    public void DisplayToast(
        string message,
        TimeSpan? duration = null,
        ToastType toastType = ToastType.Primary,
        string id = "",
        bool overrideSameId = false,
        string? title = null)
    {
        DisplayToast(builder => builder.AddContent(1, message), duration, toastType, id, overrideSameId, title);
    }

    public void DisplayToast(
        RenderFragment content,
        TimeSpan? duration = null,
        ToastType toastType = ToastType.Primary,
        string id = "",
        bool overrideSameId = false,
        string? title = null)
    {
        OnToastRequested?.Invoke(this, new(id, Guid.NewGuid(), content, duration, toastType, overrideSameId, title));
    }
}
