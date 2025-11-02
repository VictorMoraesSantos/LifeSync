using Core.Domain.Repositories;
using Gym.Domain.Entities;
using Gym.Domain.Filters;

namespace Gym.Domain.Repositories
{
    public interface IRoutineRepository : IRepository<Routine, int, RoutineQueryFilter>
    {
    }
}
