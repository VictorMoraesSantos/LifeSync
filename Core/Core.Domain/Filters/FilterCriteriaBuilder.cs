using Core.Domain.Entities;
using System.Linq.Expressions;

namespace Core.Domain.Filters
{
    public class FilterCriteriaBuilder<T, TId> where T : IBaseEntity<TId>
    {
        private readonly List<Expression<Func<T, bool>>> _predicates = new();
        private readonly IDomainQueryFilter _filter;

        public FilterCriteriaBuilder(IDomainQueryFilter filter)
        {
            _filter = filter;
        }

        public FilterCriteriaBuilder<T, TId> AddCommonFilters()
        {
            if (_filter.CreatedAt.HasValue)
                _predicates.Add(x => DateOnly.FromDateTime(x.CreatedAt) == _filter.CreatedAt.Value);

            if (_filter.UpdatedAt.HasValue)
                _predicates.Add(x => x.UpdatedAt.HasValue && DateOnly.FromDateTime(x.UpdatedAt.Value) == _filter.UpdatedAt.Value);

            if (_filter.IsDeleted.HasValue)
                _predicates.Add(x => x.IsDeleted == _filter.IsDeleted.Value);

            return this;
        }

        public FilterCriteriaBuilder<T, TId> AddIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition)
                _predicates.Add(predicate);

            return this;
        }

        public Expression<Func<T, bool>>? Build()
        {
            return BaseFilterSpecification<T, TId>.CombinePredicates(_predicates);
        }

        protected void AddPredicate(Expression<Func<T, bool>> predicate)
        {
            _predicates.Add(predicate);
        }
    }
}
