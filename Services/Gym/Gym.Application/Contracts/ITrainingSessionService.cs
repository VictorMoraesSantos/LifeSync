using Core.Application.Interfaces;
using Gym.Application.DTOs.TrainingSession;

namespace Gym.Application.Contracts
{
    public interface ITrainingSessionService
        : IReadService<TrainingSessionDTO, int>,
        ICreateService<CreateTrainingSessionDTO>,
        IUpdateService<UpdateTrainingSessionDTO>,
        IDeleteService<int>
    {
    }
}
