using System;
using System.Linq;
using System.Linq.Expressions;

namespace GeoNRage.Server
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition)
            {
                return query.Where(predicate);
            }
            else
            {
                return query;
            }
        }
    }
}
