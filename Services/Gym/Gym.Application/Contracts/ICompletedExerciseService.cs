using Core.Application.Interfaces;
using Gym.Application.DTOs.CompletedExercise;

namespace Gym.Application.Contracts
{
    public interface ICompletedExerciseService
        : IReadService<CompletedExerciseDTO, int>,
        ICreateService<CreateCompletedExerciseDTO>,
        IUpdateService<UpdateCompletedExerciseDTO>,
        IDeleteService<int>
    {
    }
}
