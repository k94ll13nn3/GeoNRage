using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Layouts.Main;

public partial class Toast
{
    private readonly CancellationTokenSource _tokenSource = new();

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