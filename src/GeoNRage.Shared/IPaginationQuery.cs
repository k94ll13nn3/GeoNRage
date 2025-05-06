namespace GeoNRage.Shared;

public interface IPaginationQuery
{
    /// <summary>
    /// The page of the result.
    /// </summary>
    int Page { get; set; }

    /// <summary>
    /// The page size of the result.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// The ordering expression.
    /// </summary>
    string? OrderBy { get; set; }

    /// <summary>
    /// The filtering expression.
    /// </summary>
    string? Filter { get; set; }
}
