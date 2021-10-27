namespace GeoNRage.App.Models;

public record TableHeader(string Title, bool CanSort, string Property);

public record UserSettings(bool AllMaps);
