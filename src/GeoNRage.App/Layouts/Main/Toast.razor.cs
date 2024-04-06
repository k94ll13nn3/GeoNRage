using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class Toast : IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private bool _disposedValue;

    [Parameter]
    public TimeSpan? Duration { get; set; }

    [Parameter]
    public EventCallback OnCloseCallback { get; set; }

    [Parameter]
    public ToastType Type { get; set; }

    [Parameter]
    public RenderFragment Content { get; set; } = null!;

    [Parameter]
    public string? Title { get; set; }

    public string NotificationClass { get; set; } = null!;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        NotificationClass = Type switch
        {
            ToastType.Error => "danger",
            ToastType.Information => "info",
            ToastType.Link => "link",
            ToastType.Warning => "warning",
            ToastType.Success => "success",
            ToastType.Primary => "primary",
            _ => "none",
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Duration is not null)
        {
            using var timer = new PeriodicTimer(Duration.Value);
            await timer.WaitForNextTickAsync(_tokenSource.Token);
            if (!_tokenSource.IsCancellationRequested)
            {
                await CloseToastAsync();
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _tokenSource.Dispose();
            }

            _disposedValue = true;
        }
    }

    private async Task CloseToastAsync()
    {
        if (!_tokenSource.IsCancellationRequested)
        {
            try
            {
                await _tokenSource.CancelAsync();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        await OnCloseCallback.InvokeAsync();
    }
}
