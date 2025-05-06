namespace GeoNRage.App.Models;

public class PaginationQuery : IPaginationQuery
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? OrderBy { get; set; }
    public string? Filter { get; set; }
}
