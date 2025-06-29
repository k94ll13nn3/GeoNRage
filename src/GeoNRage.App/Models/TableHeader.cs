namespace GeoNRage.App.Models;

public record TableHeader(string Title, string? Property)
{
    public TableHeader(string title) : this(title, null)
    {
    }

    public TableHeader(string title, string property, bool defaultSort) : this(title, property)
    {
        DefaultSort = defaultSort;
    }

    public bool DefaultSort { get; }
}
