namespace GeoNRage.App.Models;

// TODO: remove _CanSort
public record TableHeader(string Title, bool _CanSort, string? Property)
{
    public TableHeader(string title) : this(title, false, null)
    {
    }

    public TableHeader(string title, string property) : this(title, true, property)
    {
    }

    public TableHeader(string title, string property, bool defaultSort) : this(title, true, property)
    {
        DefaultSort = defaultSort;
    }

    public bool DefaultSort { get; }
}
