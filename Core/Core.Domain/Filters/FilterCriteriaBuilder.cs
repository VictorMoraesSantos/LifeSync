using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Filters
{
    public class FilterCriteriaBuilder<T, TId> where T : IBaseEntity<TId>
    {
        private readonly List<Expression<Func<T, bool>>> _predicates = new();
        private readonly IDomainQueryFilter<TId> _filter;

        public FilterCriteriaBuilder(IDomainQueryFilter<TId> filter)
        {
            _filter = filter;
        }

        public FilterCriteriaBuilder<T, TId> AddCommonFilters()
        {
            // Id (assumindo TId é int e 0 = não informado)
            if (_filter.Id is int id && id > 0)
                _predicates.Add(x => EqualityComparer<TId>.Default.Equals(x.Id, _filter.Id));

            // CreatedAt
            if (_filter.CreatedAt.HasValue)
                _predicates.Add(x => DateOnly.FromDateTime(x.CreatedAt) == _filter.CreatedAt.Value);

            // UpdatedAt
            if (_filter.UpdatedAt.HasValue)
                _predicates.Add(x => x.UpdatedAt.HasValue &&
                    DateOnly.FromDateTime(x.UpdatedAt.Value) == _filter.UpdatedAt.Value);

            // IsDeleted
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
