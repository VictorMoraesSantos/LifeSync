using Core.Domain.Repositories;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;

namespace Nutrition.Domain.Repositories
{
    public interface IDiaryRepository : IRepository<Diary, int, DiaryQueryFilter>
    {
        Task<IEnumerable<Diary?>> GetAllByUserId(int userId, CancellationToken cancellationToken);
        Task<Diary?> GetByDate(int userId, DateOnly date, CancellationToken cancellationToken);
    }
}
