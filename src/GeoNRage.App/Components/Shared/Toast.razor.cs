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

    protected override async Task OnInitializedAsync()
    {
        if (Duration is not null)
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
