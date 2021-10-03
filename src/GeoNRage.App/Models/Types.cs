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

public record ToastData(string Id, Guid UniqueId, RenderFragment Content, TimeSpan? Duration, ToastType ToastType, bool OverrideSameId, string? Title);
