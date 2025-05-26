using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Diary;
using Nutrition.Application.DTOs.Meal;

namespace Nutrition.Application.Interfaces
{
    public interface IDiaryService
        : IReadService<DiaryDTO, int>,
        ICreateService<CreateDiaryDTO>,
        IUpdateService<UpdateDiaryDTO>,
        IDeleteService<int>
    {
        Task<IEnumerable<DiaryDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<bool> AddMealToDiaryAsync(int diaryId, CreateMealDTO meal, CancellationToken cancellationToken = default);
    }
}
