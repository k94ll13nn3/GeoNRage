namespace GeoNRage.App.Models;

public interface IModal<out T>
{
    T Close();
}
