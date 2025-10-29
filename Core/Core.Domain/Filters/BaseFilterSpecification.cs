using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static BuildingBlocks.Helpers.QueryFilterBuilder;

namespace Core.Domain.Filters
{
    public class BaseFilterSpecification<T, TId> : BaseSpecification<T>
        where T : IBaseEntity<TId>
    {
        public BaseFilterSpecification(
            IDomainQueryFilter<TId> filter,
            Func<IDomainQueryFilter<TId>, Expression<Func<T, bool>>?> buildCriteria,
            Action<BaseFilterSpecification<T, TId>>? configureIncludes = null)
            : base(buildCriteria(filter))
        {
            // Configurar includes específicos da entidade
            configureIncludes?.Invoke(this);

            // Ordenação
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                ApplyDynamicOrderBy(filter.SortBy, filter.SortDesc ?? false);
            }

            // Paginação
            if (filter.Page.HasValue && filter.PageSize.HasValue)
            {
                ApplyPaging((filter.Page.Value - 1) * filter.PageSize.Value, filter.PageSize.Value);
            }
        }

        private void ApplyDynamicOrderBy(string sortBy, bool descending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortBy);
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

            if (descending)
                ApplyOrderByDescending(lambda);
            else
                ApplyOrderBy(lambda);
        }

        public static Expression<Func<T, bool>>? CombinePredicates(
            List<Expression<Func<T, bool>>> predicates)
        {
            if (!predicates.Any())
                return null;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? body = null;

            foreach (var predicate in predicates)
            {
                var visitor = new ParameterReplacer(parameter);
                var predicateBody = visitor.Visit(predicate.Body);

                body = body == null ? predicateBody : Expression.AndAlso(body, predicateBody);
            }

            return Expression.Lambda<Func<T, bool>>(body!, parameter);
        }
    }
}
