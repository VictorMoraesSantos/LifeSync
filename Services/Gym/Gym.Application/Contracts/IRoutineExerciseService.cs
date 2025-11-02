using Core.Application.Interfaces;
using Gym.Application.DTOs.RoutineExercise;

namespace Gym.Application.Contracts
{
    public interface IRoutineExerciseService
        : IReadService<RoutineExerciseDTO, int, RoutineExerciseFilterDTO>,
        ICreateService<CreateRoutineExerciseDTO>,
        IUpdateService<UpdateRoutineExerciseDTO>,
        IDeleteService<int>
    {
    }
}
