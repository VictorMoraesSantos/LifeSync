using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Repositories
{
    public class MealRepository : IMealRepository
    {
        private readonly ApplicationDbContext _context;

        public MealRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Meal?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Meal? meal = await _context.Meals
                .Include(m => m.MealFoods)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

            return meal;
        }

        public async Task<IEnumerable<Meal?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Meal?> meals = await _context.Meals
                .AsNoTracking()
                .Include(m => m.MealFoods)
                .ToListAsync(cancellationToken);

            return meals;
        }

        public async Task<IEnumerable<Meal?>> Find(Expression<Func<Meal, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Meal?> meals = await _context.Meals
                .AsNoTracking()
                .Where(predicate)
                .Include(m => m.MealFoods)
                .ToListAsync(cancellationToken);

            return meals;
        }

        public async Task Create(Meal entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Meal> entities, CancellationToken cancellationToken = default)
        {
            await _context.Meals.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Meal entity, CancellationToken cancellationToken = default)
        {
            _context.Meals.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Meal entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
