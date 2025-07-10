using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class RoutineExerciseRepository : IRoutineExerciseRepository
    {
        public Task Create(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<RoutineExercise> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoutineExercise?>> Find(Expression<Func<RoutineExercise, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoutineExercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RoutineExercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
