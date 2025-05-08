using Microsoft.AspNetCore.Components;
using Refit;

namespace GeoNRage.App.Services;

public class ToastService
{
    public event EventHandler<ToastEventArgs>? OnToastRequested;

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

    public async Task DisplayErrorToastAsync(ApiException exception, string id)
    {
        ArgumentNullException.ThrowIfNull(exception);

        ProblemDetails? error = await exception.GetContentAsAsync<ProblemDetails>();
        string message = error?.Detail ?? "Une erreur est survenue";
        DisplayToast(message, null, ToastType.Error, id, true);
    }
}
