using System.Linq.Expressions;

namespace BuildingBlocks.Helpers
{
    public static class QueryFilterBuilder
    {
        // Sobrecarga para value types nullable (int?, DateOnly?, enums, etc)
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

        // Sobrecarga para propriedades nullable (DateTime?, bool?, etc)
        public static IQueryable<T> ApplyFilter<T, TValue>(
            this IQueryable<T> query,
            TValue? filterValue,
            Expression<Func<T, TValue?>> propertySelector)
            where TValue : struct
        {
            if (!filterValue.HasValue)
                return query;

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            var constant = Expression.Constant(filterValue, typeof(TValue?));
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return query.Where(lambda);
        }

        // String contains - CORRIGIDO: agora traduz corretamente para SQL
        public static IQueryable<T> ApplyStringContains<T>(
            this IQueryable<T> query,
            string? filterValue,
            Expression<Func<T, string>> propertySelector)
        {
            if (string.IsNullOrWhiteSpace(filterValue))
                return query;

            // Constrói expressão que será traduzida para SQL LIKE
            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            var constant = Expression.Constant(filterValue, typeof(string));
            
            // Método Contains que EF Core traduz para LIKE '%value%'
            var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            var containsCall = Expression.Call(property, containsMethod, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return query.Where(lambda);
        }

        // Collection filter - RENOMEADO e corrigido (estava chamando ApplyCollectionAny mas deveria ser ApplyCollectionFilter)
        public static IQueryable<T> ApplyCollectionFilter<T, TCollection>(
            this IQueryable<T> query,
            int? filterValue,
            Expression<Func<T, IEnumerable<TCollection>>> collectionSelector,
            Expression<Func<TCollection, int>> propertySelector)
        {
            if (!filterValue.HasValue)
                return query;

            var parameter = collectionSelector.Parameters[0];
            var collection = collectionSelector.Body;
            
            var itemParameter = propertySelector.Parameters[0];
            var itemProperty = propertySelector.Body;
            var constant = Expression.Constant(filterValue.Value, typeof(int));
            var equality = Expression.Equal(itemProperty, constant);
            var predicate = Expression.Lambda<Func<TCollection, bool>>(equality, itemParameter);

            // Usa Queryable.Any para traduzir para SQL EXISTS
            var anyMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TCollection));
            
            var collectionAsQueryable = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.AsQueryable),
                new[] { typeof(TCollection) },
                collection);

            var anyCall = Expression.Call(anyMethod, collectionAsQueryable, predicate);
            var lambda = Expression.Lambda<Func<T, bool>>(anyCall, parameter);

            return query.Where(lambda);
        }

        public class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            public ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameter;
            }
        }


    }
}
