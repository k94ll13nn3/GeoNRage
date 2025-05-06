namespace GeoNRage.Shared;

/// <summary>
/// The response of a query.
/// </summary>
/// <typeparam name="T">The type of the returned data</typeparam>
public sealed class PaginationResult<T>
{
    /// <summary>
    /// The page of the result.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The page size of the result.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total count of returned data.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// The returned data.
    /// </summary>
    public required IReadOnlyCollection<T> Data { get; set; } = [];
}
