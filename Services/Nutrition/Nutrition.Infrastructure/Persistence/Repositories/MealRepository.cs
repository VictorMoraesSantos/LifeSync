using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Filters;
using Nutrition.Domain.Filters.Specifications;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Persistence.Repositories
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

        public async Task<(IEnumerable<Meal> Items, int TotalCount)> FindByFilter(MealQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new MealSpecification(filter);
            IQueryable<Meal> query = _context.Meals.AsNoTracking();
            IQueryable<Meal> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Meal> finalQuery = SpecificationEvaluator.GetQuery(_context.Meals.AsNoTracking(), spec);
            IEnumerable<Meal> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
