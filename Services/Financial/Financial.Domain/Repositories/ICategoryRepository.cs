using Core.Domain.Repositories;
using Financial.Domain.Entities;

namespace Financial.Domain.Repositories
{
    public interface ICategoryRepository : IRepository<Category, int>
    {
        Task<IEnumerable<Category?>> GetAllByUserIdAsync(int userId);
    }
}
