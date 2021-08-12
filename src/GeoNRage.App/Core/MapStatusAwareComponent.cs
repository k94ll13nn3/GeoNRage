using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Core;

public abstract class MapStatusAwareComponent : ComponentBase, IDisposable
{
    private bool _disposedValue;

    [Inject]
    protected MapStatusService MapStatusService { get; set; } = null!;

    protected override void OnInitialized()
    {
        MapStatusService.MapStatusChanged += OnMapStatusChanged;
    }

    internal abstract void OnMapStatusChanged(object? sender, EventArgs e);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                MapStatusService.MapStatusChanged -= OnMapStatusChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
