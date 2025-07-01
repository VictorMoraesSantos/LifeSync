using Core.Domain.Repositories;
using Gym.Domain.Entities;

namespace Gym.Domain.Repositories
{
    public interface ICompletedExerciseRepository : IRepository<CompletedExercise, int>
    {
    }
}
