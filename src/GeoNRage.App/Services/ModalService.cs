using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public class ModalService
{
    public event EventHandler<ModalEventArgs>? OnModalRequested;

    public async Task<TOutputData> DisplayModalAsync<TModal, TOutputData>(IDictionary<string, object> parameters)
        where TModal : ComponentBase, IModal<TOutputData>
        where TOutputData : class
    {
        TaskCompletionSource<object?> tcs = new();
        OnModalRequested?.Invoke(this, new(typeof(TModal), parameters, tcs));

        object? component = await tcs.Task;

        if (component is not IModal<TOutputData> modal)
        {
            throw new InvalidOperationException("Cannot get modal result.");
        }

        return modal.Close();
    }
}
