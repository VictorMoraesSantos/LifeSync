using Core.Domain.Repositories;
using Gym.Domain.Entities;
using Gym.Domain.Filters;

namespace Gym.Domain.Repositories
{
    public interface IExerciseRepository : IRepository<Exercise, int, ExerciseQueryFilter>
    {
    }
}
