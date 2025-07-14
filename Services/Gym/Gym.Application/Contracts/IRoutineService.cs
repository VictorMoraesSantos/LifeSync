using Core.Application.Interfaces;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Contracts
{
    public interface IRoutineService
        : IReadService<RoutineDTO, int>,
        ICreateService<CreateRoutineDTO>,
        IUpdateService<UpdateRoutineDTO>,
        IDeleteService<int>
    {
    }
}
