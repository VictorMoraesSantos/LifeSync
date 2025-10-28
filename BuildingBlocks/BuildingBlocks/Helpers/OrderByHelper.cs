using System.Linq.Expressions;
using System.Reflection;

namespace BuildingBlocks.Helpers
{
    public static class OrderByHelper
    {
        public static IQueryable<T> ApplyOrderBy<T>(
            this IQueryable<T> query,
            string? sortBy,
            bool descending = false)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return query;

            var propertyAccess = Expression.Property(parameter, property);
            var conversion = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

            return descending
                ? query.OrderByDescending(lambda)
                : query.OrderBy(lambda);
        }
    }
}
