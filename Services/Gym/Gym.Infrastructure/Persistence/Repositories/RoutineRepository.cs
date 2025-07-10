using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class RoutineRepository : IRoutineRepository
    {
        public Task Create(Routine entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<Routine> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Routine entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Routine?>> Find(Expression<Func<Routine, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Routine?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Routine?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Routine entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
