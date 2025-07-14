using Core.Application.Interfaces;
using Gym.Application.DTOs.Exercise;

namespace Gym.Application.Contracts
{
    public interface IExerciseService
        : IReadService<ExerciseDTO, int>,
        ICreateService<CreateExerciseDTO>,
        IUpdateService<UpdateExerciseDTO>,
        IDeleteService<int>
    {
    }
}
