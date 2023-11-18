using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace GeoNRage.App.Layouts.Main;

public partial class ToastRender : IDisposable
{
    private bool _disposedValue;
    private readonly List<ToastEventArgs> _toasts = [];

    [Inject]
    public ToastService ToastService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public IEnumerable<ToastEventArgs> Toasts => _toasts.AsEnumerable();

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

    private void AddToast(object? sender, ToastEventArgs toast)
    {
        if (!_toasts.Exists(t => !string.IsNullOrWhiteSpace(t.Id) && t.Id == toast.Id) || toast.OverrideSameId)
        {
            _toasts.RemoveAll(t => t.Id == toast.Id);
            _toasts.Add(toast);
            StateHasChanged();
        }
    }

    private void RemoveToast(ToastEventArgs toast)
    {
        _toasts.Remove(toast);
        StateHasChanged();
    }

    private void ClearToasts(object? sender, LocationChangedEventArgs args)
    {
        _toasts.Clear();
        StateHasChanged();
    }
}
