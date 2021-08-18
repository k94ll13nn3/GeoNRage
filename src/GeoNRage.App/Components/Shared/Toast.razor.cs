using GeoNRage.App.Services;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Components.Shared;

public partial class Toast
{
    private readonly CancellationTokenSource _tokenSource = new();

    [Parameter]
    public string Message { get; set; } = null!;

    [Parameter]
    public TimeSpan? Duration { get; set; }

    [Parameter]
    public EventCallback OnCloseCallback { get; set; }

    [Parameter]
    public ToastType Type { get; set; }

    public string NotificationClass { get; set; } = null!;

    protected override void OnInitialized()
    {
        NotificationClass = Type switch
        {
            ToastType.Error => "error",
            ToastType.Information => "info",
            ToastType.Link => "link",
            ToastType.Warning => "warning",
            ToastType.Success => "success",
            _ => "primary",
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Duration is not null)
        {
            var timer = new PeriodicTimer(Duration.Value);
            await timer.WaitForNextTickAsync(_tokenSource.Token);
            if (!_tokenSource.Token.IsCancellationRequested)
            {
                await CloseToastAsync();
            }
        }
    }

    private async Task CloseToastAsync()
    {
        _tokenSource.Cancel();
        await OnCloseCallback.InvokeAsync();
    }
}
