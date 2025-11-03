using Core.Domain.Repositories;
using Financial.Domain.Entities;
using Financial.Domain.Filters;

namespace Financial.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int, CategoryQueryFilter>
    {
        Task<IEnumerable<Category?>> GetAllByUserId(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetByNameContains(string name, CancellationToken cancellationToken = default);
    }
}
