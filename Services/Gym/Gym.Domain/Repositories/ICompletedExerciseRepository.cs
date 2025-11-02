using Core.Domain.Repositories;
using Gym.Domain.Entities;
using Gym.Domain.Filters;

namespace Gym.Domain.Repositories
{
    public interface ICompletedExerciseRepository : IRepository<CompletedExercise, int, CompletedExerciseQueryFilter>
    {
    }
}
