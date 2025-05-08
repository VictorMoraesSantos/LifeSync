using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Data;
using System.Linq.Expressions;

namespace Nutrition.Infrastructure.Repositories
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
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Diary entity, CancellationToken cancellationToken = default)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}