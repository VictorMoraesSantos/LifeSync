using Core.Application.Interfaces;
using Nutrition.Application.DTOs.Diary;

namespace Nutrition.Application.Interfaces
{
    public interface IDiaryService
        : IReadService<DiaryDTO, int>,
        ICreateService<CreateDiaryDTO>,
        IUpdateService<UpdateDiaryDTO>,
        IDeleteService<int>
    {
        Task<IEnumerable<DiaryDTO>> GetAllByUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
