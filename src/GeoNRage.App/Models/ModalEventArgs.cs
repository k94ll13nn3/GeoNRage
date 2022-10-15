namespace GeoNRage.App.Models;

[AutoConstructor]
public partial class ModalEventArgs : EventArgs
{
    public Type ComponentType { get; }

    public IDictionary<string, object> Parameters { get; }

    public TaskCompletionSource<object?> Result { get; }
}
