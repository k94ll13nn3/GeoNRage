using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public class ModalService
{
    public event EventHandler<ModalEventArgs>? OnModalRequested;

    public async Task<TOutputData> DisplayModalAsync<TModal, TOutputData>(Dictionary<string, object> parameters, ModalOptions options)
        where TModal : ComponentBase, IModal<TOutputData>
        where TOutputData : class
    {
        TaskCompletionSource<object?> tcs = new();
        OnModalRequested?.Invoke(this, new(typeof(TModal), parameters, tcs, options));

        object? component = await tcs.Task;

        if (component is not IModal<TOutputData> modal)
        {
            throw new InvalidOperationException("Cannot get modal result.");
        }

        return modal.Close();
    }

    public async Task DisplayModalAsync<TModal>(Dictionary<string, object> parameters, ModalOptions options)
        where TModal : ComponentBase, IModal
    {
        TaskCompletionSource<object?> tcs = new();
        OnModalRequested?.Invoke(this, new(typeof(TModal), parameters, tcs, options));

        object? component = await tcs.Task;

        if (component is not IModal)
        {
            throw new InvalidOperationException("Cannot get modal result.");
        }

        return;
    }
}
