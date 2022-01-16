using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Core;

public abstract class UserSettingsAwareComponent : ComponentBase, IDisposable
{
    private bool _disposedValue;

    [Inject]
    protected UserSettingsService UserSettingsService { get; set; } = null!;

    protected override void OnInitialized()
    {
        UserSettingsService.SettingsChanged += OnSettingsChanged;
    }

    internal abstract void OnSettingsChanged(object? sender, UserSettingsEventArgs e);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                UserSettingsService.SettingsChanged -= OnSettingsChanged;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
