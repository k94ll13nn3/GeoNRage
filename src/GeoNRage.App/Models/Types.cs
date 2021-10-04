using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Models;

public record TableHeader(string Title, bool CanSort, string Property);

public record UserSettings(bool AllMaps);

public enum ToastType
{
    Primary = 0,
    Information = 1,
    Link = 2,
    Success = 3,
    Warning = 4,
    Error = 5,
}

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

    public string Id { get; set; }

    public Guid UniqueId { get; set; }

    public RenderFragment Content { get; set; }

    public TimeSpan? Duration { get; set; }

    public ToastType ToastType { get; set; }

    public bool OverrideSameId { get; set; }

    public string? Title { get; set; }
}
