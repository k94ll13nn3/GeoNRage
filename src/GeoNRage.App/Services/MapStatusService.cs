namespace GeoNRage.App.Services;

public class MapStatusService
{
    public bool AllMaps { get; private set; }

    public event EventHandler? MapStatusChanged;

    public void SetMapStatus(bool status)
    {
        if (AllMaps != status)
        {
            AllMaps = status;
            MapStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
