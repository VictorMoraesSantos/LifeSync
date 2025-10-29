using Core.Domain.Entities;
using Core.Domain.Filters;
using System.Linq.Expressions;

namespace Core.Domain.Repositories
{
    public interface IRepository<T, TId, TFilter>
        where T : IBaseEntity<TId>
        where TFilter : IDomainQueryFilter<TId?>
    {
        Task<T?> GetById(TId id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T?>> GetAll(CancellationToken cancellationToken = default);
        Task<IEnumerable<T?>> Find(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<(IEnumerable<T> Items, int TotalCount)> FindByFilter(TFilter filter, CancellationToken cancellationToken = default);
        Task Create(T entity, CancellationToken cancellationToken = default);
        Task CreateRange(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task Update(T entity, CancellationToken cancellationToken = default);
        Task Delete(T entity, CancellationToken cancellationToken = default);
    }
}
