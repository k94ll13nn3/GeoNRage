using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Models;

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
