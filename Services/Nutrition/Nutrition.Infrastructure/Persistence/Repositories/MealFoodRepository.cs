using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Persistence.Repositories
{
    public class MealFoodRepository : IMealFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public MealFoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MealFood?> GetById(int id, CancellationToken cancellationToken = default)
        {
            MealFood? food = await _context.MealFoods
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            return food;
        }

        public async Task<IEnumerable<MealFood?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<MealFood?> foods = await _context.MealFoods
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return foods;
        }

        public async Task<IEnumerable<MealFood?>> Find(Expression<Func<MealFood, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<MealFood?> foods = await _context.MealFoods
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return foods;
        }

        public async Task Create(MealFood entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<MealFood> entities, CancellationToken cancellationToken = default)
        {
            await _context.MealFoods.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(MealFood entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(MealFood entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}