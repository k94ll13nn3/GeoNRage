using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class PopupRender : IDisposable
{
    private bool _disposedValue;

    [Inject]
    public PopupService PopupService { get; set; } = null!;

    public bool DisableButtons { get; set; }

    public PopupEventArgs Args { get; set; } = null!;

    public bool IsOpen { get; set; }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        if (PopupService is null)
        {
            throw new InvalidOperationException("Cannot start Popup component.");
        }

        PopupService.OnPopupUpdated += Update;
        PopupService.OnPopupHidden += OnPopupHidden;
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing && PopupService is not null)
            {
                PopupService.OnPopupUpdated -= Update;
                PopupService.OnPopupHidden -= OnPopupHidden;
            }

            _disposedValue = true;
        }
    }

    private void Update(object? sender, PopupEventArgs e)
    {
        DisableButtons = false;
        Args = e;
        IsOpen = true;
        StateHasChanged();
    }

    private void OnValidate()
    {
        HidePopup();
        Args.OnOnClick?.Invoke();
    }

    private void HidePopup()
    {
        IsOpen = false;
        StateHasChanged();
    }

    private void OnPopupHidden(object? sender, EventArgs e)
    {
        HidePopup();
    }
}
