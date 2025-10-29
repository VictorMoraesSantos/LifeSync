using Core.Domain.Filters;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec) where T : class
        {
            var query = inputQuery;

            // Aplica o critério
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // Aplica os includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Aplica ordenação
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            // Aplica paginação
            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }
}
