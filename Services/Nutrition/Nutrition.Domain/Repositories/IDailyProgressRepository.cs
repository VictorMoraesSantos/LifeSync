using Core.Domain.Repositories;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;

namespace Nutrition.Domain.Repositories
{
    public interface IDailyProgressRepository : IRepository<DailyProgress, int, DailyProgressQueryFilter>
    {
        Task<IEnumerable<DailyProgress?>> GetAllByUserId(int userId, CancellationToken cancellationToken);
        Task<DailyProgress?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);
    }
}
