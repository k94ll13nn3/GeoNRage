namespace GeoNRage.App.Models.Modal;

[AutoConstructor]
public partial class ModalEventArgs : EventArgs
{
    public Type ComponentType { get; }

    public IDictionary<string, object?> Parameters { get; }

    public TaskCompletionSource<object?> Result { get; }

    public ModalOptions Options { get; }
}
