using Core.Infrastructure.Persistence.Specifications;
using Financial.Domain.Entities;
using Financial.Domain.Filters;
using Financial.Domain.Filters.Specifications;
using Financial.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Category entity, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Category> entities, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddRangeAsync(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Category entity, CancellationToken cancellationToken = default)
        {
            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category?>> Find(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Category?> entities = await _context.Categories
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<(IEnumerable<Category> Items, int TotalCount)> FindByFilter(CategoryQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new CategorySpecification(filter);
            IQueryable<Category> query = _context.Categories.AsNoTracking();
            IQueryable<Category> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Category> finalQuery = SpecificationEvaluator.GetQuery(_context.Categories.AsNoTracking(), spec);
            IEnumerable<Category> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<Category?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Category?> entities = await _context.Categories
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Category?>> GetAllByUserId(int userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<Category?> entities = await _context.Categories
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return entities;
        }

        public async Task<Category?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Category? entity = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<Category?>> GetByNameContains(string name, CancellationToken cancellationToken = default)
        {
            IEnumerable<Category> entities = await _context.Categories
                .Where(c => c.Name.Contains(name))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Update(Category entity, CancellationToken cancellationToken = default)
        {
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
