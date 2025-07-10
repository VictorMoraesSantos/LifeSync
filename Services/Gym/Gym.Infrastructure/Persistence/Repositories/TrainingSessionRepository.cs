using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class TrainingSessionRepository : ITrainingSessionRepository
    {
        public Task Create(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<TrainingSession> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TrainingSession?>> Find(Expression<Func<TrainingSession, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TrainingSession?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TrainingSession?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
