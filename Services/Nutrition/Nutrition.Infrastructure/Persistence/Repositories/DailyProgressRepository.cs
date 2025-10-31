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
    public class DailyProgressRepository : IDailyProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DailyProgress?> GetById(int id, CancellationToken cancellationToken = default)
        {
            DailyProgress? dailyProgress = await _context.DailyProgresses
                .Include(dp => dp.Goal)
                .FirstOrDefaultAsync(dp => dp.Id == id, cancellationToken);

            return dailyProgress;
        }

        public async Task<IEnumerable<DailyProgress?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<DailyProgress> dailyProgresses = await _context.DailyProgresses
                .AsNoTracking()
                .Include(dp => dp.Goal)
                .ToListAsync(cancellationToken);

            return dailyProgresses;
        }

        public async Task<IEnumerable<DailyProgress?>> Find(Expression<Func<DailyProgress, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<DailyProgress?> dailyProgresses = await _context.DailyProgresses
                .AsNoTracking()
                .Include(dp => dp.Goal)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return dailyProgresses;
        }

        public async Task Create(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            await _context.DailyProgresses.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<DailyProgress> entities, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(DailyProgress entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<DailyProgress?>> GetAllByUserId(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<DailyProgress?> dailyProgresses = await _context.DailyProgresses
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .Include(d => d.Goal)
                .ToListAsync(cancellationToken);

            return dailyProgresses;
        }

        public async Task<DailyProgress?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            DailyProgress? dailyProgress = await _context.DailyProgresses
                .Include(dp => dp.Goal)
                .FirstOrDefaultAsync(dp => dp.UserId == userId && dp.Date == date, cancellationToken);
            return dailyProgress;
        }

        public async Task<(IEnumerable<DailyProgress> Items, int TotalCount)> FindByFilter(DailyProgressQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new DailyProgressSpecification(filter);
            IQueryable<DailyProgress> query = _context.DailyProgresses.AsNoTracking();
            IQueryable<DailyProgress> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<DailyProgress> finalQuery = SpecificationEvaluator.GetQuery(_context.DailyProgresses.AsNoTracking(), spec);
            IEnumerable<DailyProgress> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}