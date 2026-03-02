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
    public class FoodRepository : IFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public FoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Food?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Food? food = await _context.Foods
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
            return food;
        }

        public async Task<IEnumerable<Food?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Food?> foods = await _context.Foods
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return foods;
        }

        public async Task<IEnumerable<Food?>> Find(Expression<Func<Food, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Food?> foods = await _context.Foods
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();

            return foods;
        }

        public async Task<(IEnumerable<Food> Items, int TotalCount)> FindByFilter(FoodQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new FoodSpecification(filter);
            IQueryable<Food> query = _context.Foods.AsNoTracking();
            IQueryable<Food> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Food> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<Food> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task Create(Food entity, CancellationToken cancellationToken = default)
        {
            await _context.Foods.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Food> entities, CancellationToken cancellationToken = default)
        {
            await _context.Foods.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Food entity, CancellationToken cancellationToken = default)
        {
            _context.Foods.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Food entity, CancellationToken cancellationToken = default)
        {
            _context.Foods.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
