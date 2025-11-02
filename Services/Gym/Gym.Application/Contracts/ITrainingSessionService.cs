using BuildingBlocks.Results;
using Core.Application.Interfaces;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Contracts
{
    public interface ITrainingSessionService
        : IReadService<TrainingSessionDTO, int, TrainingSessionFilterDTO>,
        ICreateService<CreateTrainingSessionDTO>,
        IUpdateService<UpdateTrainingSessionDTO>,
        IDeleteService<int>
    {
        Task<Result<IEnumerable<TrainingSessionDTO?>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default!);
    }
}
