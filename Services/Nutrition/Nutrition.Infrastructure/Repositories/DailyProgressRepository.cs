using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Repositories
{
    public class DailyProgressRepository : IDailyProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<DailyProgress?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DailyProgress?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DailyProgress?>> Find(Expression<Func<DailyProgress, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Create(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<DailyProgress> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}