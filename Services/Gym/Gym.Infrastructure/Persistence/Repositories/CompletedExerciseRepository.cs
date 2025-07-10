using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class CompletedExerciseRepository : ICompletedExerciseRepository
    {
        public Task Create(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<CompletedExercise> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompletedExercise?>> Find(Expression<Func<CompletedExercise, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompletedExercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CompletedExercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
