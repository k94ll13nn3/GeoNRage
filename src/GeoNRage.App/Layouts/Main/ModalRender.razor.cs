using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace GeoNRage.App.Layouts.Main;

public partial class ModalRender : IDisposable
{
    private ModalEventArgs? _eventArgs;
    private bool _isOpen;
    private bool _disposedValue;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public void Cancel()
    {
        Close(new ModalResult(true));
    }

    public void Close(object? result = null)
    {
        _isOpen = false;
        StateHasChanged();
        _eventArgs?.Result.SetResult(result);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        if (ModalService is null)
        {
            throw new InvalidOperationException("Cannot start Modal component.");
        }

        ModalService.OnModalRequested += ShowModal;
        NavigationManager.LocationChanged += Close;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && ModalService is not null)
            {
                ModalService.OnModalRequested -= ShowModal;
                NavigationManager.LocationChanged -= Close;
            }

            _disposedValue = true;
        }
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            Cancel();
        }
    }

    private void ShowModal(object? sender, ModalEventArgs e)
    {
        _eventArgs = e;
        _isOpen = true;
        StateHasChanged();
    }

    private void Close(object? sender, LocationChangedEventArgs args)
    {
        Cancel();
    }
}
