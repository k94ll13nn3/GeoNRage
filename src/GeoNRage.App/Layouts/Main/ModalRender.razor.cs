using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace GeoNRage.App.Layouts.Main;

public partial class ModalRender : IDisposable
{
    private Type? _componentType;
    private IDictionary<string, object>? _parameters;
    private Guid? _guid;
    private bool _isOpen;
    private bool _disposedValue;
    private DynamicComponent? _componentRef;

    [Inject]
    public ModalService ModalService { get; set; } = null!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

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
            Close();
        }
    }

    private void ShowModal(object? sender, ModalEventArgs e)
    {
        _componentType = e.ComponentType;
        _parameters = e.Parameters;
        _guid = e.ComponentGuid;
        _isOpen = true;
        StateHasChanged();
    }

    private void Close()
    {
        _isOpen = false;
        object? result = (_componentRef?.Instance as IModal)?.Close();
        if (_guid is not null && result is not null)
        {
            ModalService.SetResult(_guid.Value, result);
        }

        _guid = null;
        _componentType = null;
        _parameters = null;
        StateHasChanged();
    }

    private void Close(object? sender, LocationChangedEventArgs args)
    {
        Close();
    }
}
