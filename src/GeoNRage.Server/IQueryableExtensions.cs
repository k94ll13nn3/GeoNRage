using System.Linq.Expressions;
using Gridify;
using Gridify.EntityFramework;

namespace GeoNRage.Server;

internal static class IQueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static async Task<PaginationResult<T>> PaginateAsync<T>(
        this IQueryable<T> query,
        IGridifyQuery gridifyQuery,
        IGridifyMapper<T> mapper,
        CancellationToken cancellationToken)
    {
        Paging<T> pagingResult = await query.GridifyAsync(gridifyQuery, cancellationToken, mapper);

        return new PaginationResult<T>()
        {
            Count = pagingResult.Count,
            Data = [.. pagingResult.Data],
            Page = gridifyQuery.Page,
            PageSize = gridifyQuery.PageSize
        };
    }
}
