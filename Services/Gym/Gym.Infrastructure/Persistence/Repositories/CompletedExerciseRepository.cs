using Core.Infrastructure.Persistence.Specifications;
using Gym.Domain.Entities;
using Gym.Domain.Filters;
using Gym.Domain.Filters.Specifications;
using Gym.Domain.Repositories;
using Gym.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class CompletedExerciseRepository : ICompletedExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public CompletedExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            await _context.CompletedExercises.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<CompletedExercise> entities, CancellationToken cancellationToken = default)
        {
            await _context.CompletedExercises.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            _context.CompletedExercises.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompletedExercise?>> Find(
            Expression<Func<CompletedExercise, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var entities = await _context.CompletedExercises
                .Where(predicate)
                .Include(re => re.RoutineExercise)
                .Include(ts => ts.TrainingSession)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<(IEnumerable<CompletedExercise> Items, int TotalCount)> FindByFilter(CompletedExerciseQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new CompletedExerciseSpecification(filter);
            IQueryable<CompletedExercise> query = _context.CompletedExercises.AsNoTracking();
            IQueryable<CompletedExercise> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<CompletedExercise> finalQuery = SpecificationEvaluator.GetQuery(_context.CompletedExercises.AsNoTracking(), spec);
            IEnumerable<CompletedExercise> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<CompletedExercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.CompletedExercises
                .Include(re => re.RoutineExercise)
                .Include(ts => ts.TrainingSession)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<CompletedExercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.CompletedExercises
                .Include(re => re.RoutineExercise)
                .Include(ts => ts.TrainingSession)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return entity;
        }

        public async Task Update(CompletedExercise entity, CancellationToken cancellationToken = default)
        {
            _context.CompletedExercises.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}