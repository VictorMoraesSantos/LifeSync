using System.Linq.Expressions;

namespace BuildingBlocks.Helpers
{
    public static class QueryFilterBuilder
    {
        public static IQueryable<T> ApplyFilter<T, TValue>(
            this IQueryable<T> query,
            TValue? filterValue,
            Expression<Func<T, TValue>> propertySelector)
            where TValue : struct
        {
            if (!filterValue.HasValue)
                return query;

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            var constant = Expression.Constant(filterValue.Value, typeof(TValue));
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return query.Where(lambda);
        }

        public static IQueryable<T> ApplyStringContains<T>(
            this IQueryable<T> query,
            string? filterValue,
            Expression<Func<T, string>> propertySelector)
        {
            if (string.IsNullOrEmpty(filterValue))
                return query;

            var searchValue = filterValue.ToLower().Trim();
            var parameter = propertySelector.Parameters[0];
            var body = Expression.Call(
                Expression.Call(propertySelector.Body, "ToLower", null),
                "Contains",
                null,
                Expression.Constant(searchValue));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return query.Where(lambda);
        }
    }
}
