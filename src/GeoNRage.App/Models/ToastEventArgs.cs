using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Models;

[AutoConstructor]
public partial class ToastEventArgs : EventArgs
{
    public string Id { get; }

    public Guid UniqueId { get; }

    public RenderFragment Content { get; }

    public TimeSpan? Duration { get; }

    public ToastType ToastType { get; }

    public bool OverrideSameId { get; }

    public string? Title { get; }
}
