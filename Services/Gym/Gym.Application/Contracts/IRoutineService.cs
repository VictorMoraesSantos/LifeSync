using Core.Application.Interfaces;
using Gym.Application.DTOs.Routine;

namespace Gym.Application.Contracts
{
    public interface IRoutineService
        : IReadService<RoutineDTO, int, RoutineFilterDTO>,
        ICreateService<CreateRoutineDTO>,
        IUpdateService<UpdateRoutineDTO>,
        IDeleteService<int>
    {
    }
}
