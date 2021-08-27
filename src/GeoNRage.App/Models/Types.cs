using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Models;

public record TableHeader(string Title, bool CanSort, string Property);

public record UserSettings(bool AllMaps);

public enum ToastType
{
    Primary = 0,
    Information,
    Link,
    Success,
    Warning,
    Error,
}

public record ToastData(string Id, RenderFragment Content, TimeSpan? Duration, ToastType ToastType);
