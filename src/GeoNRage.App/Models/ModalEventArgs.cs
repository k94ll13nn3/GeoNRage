namespace GeoNRage.App.Models;

[AutoConstructor]
public partial class ModalEventArgs : EventArgs
{
    public Guid ComponentGuid { get; }

    public Type ComponentType { get; }

    public IDictionary<string, object> Parameters { get; }
}
