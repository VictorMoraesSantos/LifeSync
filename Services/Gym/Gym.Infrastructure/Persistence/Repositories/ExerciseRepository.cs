using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        public Task Create(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<Exercise> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Exercise?>> Find(Expression<Func<Exercise, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Exercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Exercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Exercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
