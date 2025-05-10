using Core.Domain.Repositories;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Repositories
{
    public interface IDiaryRepository : IRepository<Diary, int>
    {
        Task<IEnumerable<Diary?>> GetAllByUserId(int userId, CancellationToken cancellationToken);
        Task<Diary?> GetByDate(int userId, DateOnly date, CancellationToken cancellationToken);
    }
}
