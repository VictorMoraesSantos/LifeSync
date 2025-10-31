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

        public async Task<(IEnumerable<MealFood> Items, int TotalCount)> FindByFilter(MealFoodQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new MealFoodSpecification(filter);
            IQueryable<MealFood> query = _context.MealFoods.AsNoTracking();
            IQueryable<MealFood> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<MealFood> finalQuery = SpecificationEvaluator.GetQuery(_context.MealFoods.AsNoTracking(), spec);
            IEnumerable<MealFood> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}