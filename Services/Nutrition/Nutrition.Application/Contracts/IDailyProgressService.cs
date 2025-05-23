using Core.Application.Interfaces;
using Nutrition.Application.DTOs.DailyProgress;

namespace Nutrition.Application.Interfaces
{
    public interface IDailyProgressService
        : IReadService<DailyProgressDTO, int>,
        ICreateService<CreateDailyProgressDTO>,
        IUpdateService<UpdateDailyProgressDTO>,
        IDeleteService<int>
    {
        Task<IEnumerable<DailyProgressDTO>> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task SetGoalAsync(int id, DailyGoalDTO goalDto, CancellationToken cancellationToken);
        Task SetConsumedAsync(int id, int calories, int liquidsMl, CancellationToken cancellationToken);
    }
}
