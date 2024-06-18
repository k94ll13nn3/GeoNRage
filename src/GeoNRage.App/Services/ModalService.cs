using GeoNRage.App.Components;
using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public class ModalService
{
    public event EventHandler<ModalEventArgs>? OnModalRequested;

    public async Task<ModalResult<TOutputData>> DisplayModalAsync<TModal, TOutputData>(Dictionary<string, object?> parameters, ModalOptions options)
        where TModal : ComponentBase, IModal
    {
        TaskCompletionSource<object?> tcs = new();
        OnModalRequested?.Invoke(this, new(typeof(TModal), parameters, tcs, options));

        object? taskResult = await tcs.Task;

        return taskResult switch
        {
            ModalResult { Cancelled: true } => new ModalResult<TOutputData>(default, true),
            TOutputData result => new ModalResult<TOutputData>(result, false),
            _ => throw new InvalidOperationException("Cannot get modal result."),
        };
    }

    public async Task<ModalResult> DisplayModalAsync<TModal>(Dictionary<string, object?> parameters, ModalOptions options)
        where TModal : ComponentBase, IModal
    {
        TaskCompletionSource<object?> tcs = new();
        OnModalRequested?.Invoke(this, new(typeof(TModal), parameters, tcs, options));

        object? taskResult = await tcs.Task;

        return taskResult switch
        {
            ModalResult { Cancelled: true } result => result,
            _ => new ModalResult(false),
        };
    }

    public async Task<ModalResult> DisplayOkCancelPopupAsync(string title, string message)
    {
        Dictionary<string, object?> parameters = new()
        {
            [nameof(OkCancelModal.Message)] = message,
            [nameof(OkCancelModal.Title)] = title
        };

        return await DisplayModalAsync<OkCancelModal>(parameters, ModalOptions.Default);
    }

    public async Task<ModalResult> DisplayLoaderAsync(Func<Task> action)
    {
        return await DisplayModalAsync<LoaderModal>(new() { [nameof(LoaderModal.Action)] = action }, ModalOptions.Default with { Size = ModalSize.Small });
    }
}
