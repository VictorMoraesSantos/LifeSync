using Core.Domain.Repositories;
using Nutrition.Domain.Entities;

namespace Nutrition.Domain.Repositories
{
    public interface IDailyProgressRepository : IRepository<DailyProgress, int>
    {
        Task<IEnumerable<DailyProgress?>> GetAllByUserId(int userId, CancellationToken cancellationToken);
        Task<DailyProgress?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);
    }
}
