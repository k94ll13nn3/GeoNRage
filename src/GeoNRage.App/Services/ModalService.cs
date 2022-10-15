using Microsoft.AspNetCore.Components;

namespace GeoNRage.App.Services;

public class ModalService
{
    private readonly Dictionary<Guid, object> _results = new();

    public event EventHandler<ModalEventArgs>? OnModalRequested;

    public async Task<TOutputData> DisplayModalAsync<TModal, TOutputData>(IDictionary<string, object> parameters)
        where TModal : ComponentBase, IModal
        where TOutputData : class
    {
        var componentGuid = Guid.NewGuid();
        OnModalRequested?.Invoke(this, new(componentGuid, typeof(TModal), parameters));

        while (!_results.ContainsKey(componentGuid))
        {
            await Task.Delay(100);
        }

        if (_results[componentGuid] is not TOutputData result)
        {
            throw new InvalidOperationException("Error when getting modal return value.");
        }

        _results.Remove(componentGuid);

        return result;
    }

    public void SetResult(Guid componentGuid, object result)
    {
        _results[componentGuid] = result;
    }
}
