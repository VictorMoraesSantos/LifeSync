using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Repositories
{
    public class MealFoodRepository : IMealFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public MealFoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<MealFood?> GetById(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MealFood?>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MealFood?>> Find(Expression<Func<MealFood, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Create(MealFood entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CreateRange(IEnumerable<MealFood> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Update(MealFood entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Delete(MealFood entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}