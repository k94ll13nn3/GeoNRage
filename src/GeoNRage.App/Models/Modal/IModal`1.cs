namespace GeoNRage.App.Models.Modal;

public interface IModal<out T> : IModal
{
    T Close();
}
