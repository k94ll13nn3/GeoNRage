using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace GeoNRage.App.Components.Shared;

public record ToastData(Guid Id, string Message);

public partial class ToastRender : IDisposable
{
    private bool _disposedValue;

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public List<ToastData> Toasts { get; set; } = new();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        if (ToastService is null)
        {
            throw new InvalidOperationException("Cannot start Toast component.");
        }

        ToastService.OnToastRequested += AddToast;
        NavigationManager.LocationChanged += ClearToasts;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && ToastService is not null)
            {
                ToastService.OnToastRequested -= AddToast;
                NavigationManager.LocationChanged -= ClearToasts;
            }

            _disposedValue = true;
        }
    }

    private void AddToast(object? sender, string e)
    {
        Toasts.Add(new(Guid.NewGuid(), e));
        StateHasChanged();
    }

    private void RemoveToast(ToastData toast)
    {
        Toasts.Remove(toast);
        StateHasChanged();
    }

    private void ClearToasts(object? sender, LocationChangedEventArgs args)
    {
        Toasts.Clear();
        StateHasChanged();
    }
}
