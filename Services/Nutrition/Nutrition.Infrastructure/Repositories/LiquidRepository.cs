using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Repositories
{
    public class LiquidRepository : ILiquidRepository
    {
        private readonly ApplicationDbContext _context;

        public LiquidRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Liquid?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Liquid?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Liquid?>> Find(Expression<Func<Liquid, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Create(Liquid entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<Liquid> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(Liquid entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(Liquid entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}