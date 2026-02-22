using Core.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;
using static BuildingBlocks.Helpers.QueryFilterBuilder;

namespace Core.Domain.Filters
{
    /// <summary>
    /// Base specification that encapsulates query configuration consumed by
    /// <c>SpecificationEvaluator</c>: criteria, includes, ordering and paging.
    /// </summary>
    public abstract class Specification<T> where T : class
    {
        /// <summary>The combined WHERE predicate. Null means no filter.</summary>
        public Expression<Func<T, bool>>? Criteria { get; internal set; }

        /// <summary>Navigation properties to eagerly load via <c>Include</c>.</summary>
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        /// <summary>Ascending sort expression.</summary>
        public Expression<Func<T, object>>? OrderBy { get; internal set; }

        /// <summary>Descending sort expression.</summary>
        public Expression<Func<T, object>>? OrderByDescending { get; internal set; }

        /// <summary>Number of records to skip (pagination).</summary>
        public int Skip { get; internal set; }

        /// <summary>Number of records to take (pagination).</summary>
        public int Take { get; internal set; }

        /// <summary>Indicates whether pagination was configured.</summary>
        public bool IsPagingEnabled { get; internal set; }

        /// <summary>Registers a navigation property for eager loading.</summary>
        protected void AddInclude(Expression<Func<T, object>> include) =>
            Includes.Add(include);

        /// <summary>Applies an ascending sort expression.</summary>
        protected void ApplyOrderBy(Expression<Func<T, object>> expr) =>
            OrderBy = expr;

        /// <summary>Applies a descending sort expression.</summary>
        protected void ApplyOrderByDescending(Expression<Func<T, object>> expr) =>
            OrderByDescending = expr;

        /// <summary>
        /// Configures pagination. <paramref name="skip"/> is the number of records
        /// to bypass; <paramref name="take"/> is the page size.
        /// </summary>
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }

    /// <summary>
    /// Domain-aware specification with built-in predicate composition, audit filters,
    /// dynamic ordering and fluent <see cref="AddIf"/> support.
    /// Replaces <c>BaseFilterSpecification</c> and <c>FilterCriteriaBuilder</c>.
    /// </summary>
    public abstract class Specification<T, TId> : Specification<T>
        where T : class, IBaseEntity<TId>
    {
        private readonly List<Expression<Func<T, bool>>> _predicates = new();

        /// <summary>
        /// Adds a filter predicate unconditionally, merging it with existing predicates via AND.
        /// </summary>
        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            _predicates.Add(criteria);
            Criteria = CombinePredicates(_predicates);
        }

        /// <summary>
        /// Adds a filter predicate only when <paramref name="condition"/> is true.
        /// </summary>
        protected void AddIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition) AddCriteria(predicate);
        }

        /// <summary>
        /// Applies common audit filters (CreatedAt, UpdatedAt, IsDeleted), dynamic ordering
        /// and pagination from <paramref name="filter"/>. Call at the start of the constructor.
        /// </summary>
        protected void ApplyBaseFilters(IDomainQueryFilter filter)
        {
            if (filter.CreatedAt.HasValue)
                AddCriteria(x => DateOnly.FromDateTime(x.CreatedAt) == filter.CreatedAt.Value);

            if (filter.UpdatedAt.HasValue)
                AddCriteria(x => x.UpdatedAt.HasValue
                    && DateOnly.FromDateTime(x.UpdatedAt.Value) == filter.UpdatedAt.Value);

            if (filter.IsDeleted.HasValue)
                AddCriteria(x => x.IsDeleted == filter.IsDeleted.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
                ApplyDynamicOrderBy(filter.SortBy, filter.SortDesc ?? false);

            if (filter.Page.HasValue && filter.PageSize.HasValue)
                ApplyPaging((filter.Page.Value - 1) * filter.PageSize.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// Builds a dynamic OrderBy/OrderByDescending expression from a property name string.
        /// Throws <see cref="ArgumentException"/> if the property does not exist on <typeparamref name="T"/>.
        /// </summary>
        private void ApplyDynamicOrderBy(string sortBy, bool descending)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(sortBy, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new ArgumentException(
                    $"Property '{sortBy}' does not exist on entity '{entityType.Name}'. " +
                    $"Available properties: {string.Join(", ", entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name))}");

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var conversion = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

            if (descending) ApplyOrderByDescending(lambda);
            else ApplyOrderBy(lambda);
        }

        private static Expression<Func<T, bool>>? CombinePredicates(List<Expression<Func<T, bool>>> predicates)
        {
            if (predicates.Count == 0) return null;

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
