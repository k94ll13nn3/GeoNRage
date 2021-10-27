using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Models;

public class ToastEventArgs : EventArgs
{
    public ToastEventArgs(string id, Guid uniqueId, RenderFragment content, TimeSpan? duration, ToastType toastType, bool overrideSameId, string? title)
    {
        Id = id;
        UniqueId = uniqueId;
        Content = content;
        Duration = duration;
        ToastType = toastType;
        OverrideSameId = overrideSameId;
        Title = title;
    }

    public string Id { get; }

    public Guid UniqueId { get; }

    public RenderFragment Content { get; }

    public TimeSpan? Duration { get; }

    public ToastType ToastType { get; }

    public bool OverrideSameId { get; }

    public string? Title { get; }
}
