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
    public class DiaryRepository : IDiaryRepository
    {
        private readonly ApplicationDbContext _context;

        public DiaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Diary?> GetById(int id, CancellationToken cancellationToken = default)
        {
            Diary? diary = await _context.Diaries
                .AsNoTracking()
                .Include(d => d.Meals)
                    .ThenInclude(m => m.MealFoods)
                .Include(d => d.Liquids)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            return diary;
        }

        public async Task<IEnumerable<Diary?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<Diary?> diaries = await _context.Diaries
                .AsNoTracking()
                .Include(d => d.Meals)
                    .ThenInclude(m => m.MealFoods)
                .Include(d => d.Liquids)
                .ToListAsync(cancellationToken);

            return diaries;
        }

        public async Task<IEnumerable<Diary>> GetAllByUserId(int userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<Diary?> diaries = await _context.Diaries
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.MealFoods)
                .Include(d => d.Liquids)
                .ToListAsync(cancellationToken);

            return diaries;
        }

        public async Task<Diary?> GetByDate(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            Diary? diary = await _context.Diaries
                .AsNoTracking()
                .Where(d => d.UserId == userId && d.Date == date)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.MealFoods)
                .Include(d => d.Liquids)
                .FirstOrDefaultAsync(d => d.Date == date);

            return diary;
        }

        public async Task<IEnumerable<Diary?>> Find(Expression<Func<Diary, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<Diary?> diaries = await _context.Diaries
                .AsNoTracking()
                .Where(predicate)
                .Include(d => d.Meals)
                    .ThenInclude(m => m.MealFoods)
                .Include(d => d.Liquids)
                .ToListAsync(cancellationToken);

            return diaries;
        }

        public async Task Create(Diary entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Diary> entities, CancellationToken cancellationToken = default)
        {
            await _context.Diaries.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(Diary entity, CancellationToken cancellationToken = default)
        {
            // Use Update to ensure EF Core tracks and inserts new child entities (meals, liquids)
            _context.Diaries.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Diary entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<Diary> Items, int TotalCount)> FindByFilter(DiaryQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new DiarySpecification(filter);
            IQueryable<Diary> query = _context.Diaries.AsNoTracking();
            IQueryable<Diary> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Diary> finalQuery = SpecificationEvaluator.GetQuery(_context.Diaries.AsNoTracking(), spec);
            IEnumerable<Diary> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}