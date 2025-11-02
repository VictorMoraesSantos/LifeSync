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
    public class TrainingSessionRepository : ITrainingSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            await _context.TrainingSessions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TrainingSession> entities, CancellationToken cancellationToken = default)
        {
            await _context.TrainingSessions.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            _context.TrainingSessions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TrainingSession?>> Find(Expression<Func<TrainingSession, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _context.TrainingSessions
                .Where(predicate)
                .Include(r => r.Routine)
                .Include(ce => ce.CompletedExercises)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<(IEnumerable<TrainingSession> Items, int TotalCount)> FindByFilter(TrainingSessionQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new TrainingSessionSpecification(filter);
            IQueryable<TrainingSession> query = _context.TrainingSessions.AsNoTracking();
            IQueryable<TrainingSession> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<TrainingSession> finalQuery = SpecificationEvaluator.GetQuery(_context.TrainingSessions.AsNoTracking(), spec);
            IEnumerable<TrainingSession> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<TrainingSession?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.TrainingSessions
                .Include(r => r.Routine)
                .Include(ce => ce.CompletedExercises)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<TrainingSession?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.TrainingSessions
                .Include(r => r.Routine)
                .Include(ce => ce.CompletedExercises)
                .AsNoTracking()
                .FirstOrDefaultAsync(ts => ts.Id == id, cancellationToken);

            return entity;
        }

        public async Task Update(TrainingSession entity, CancellationToken cancellationToken = default)
        {
            _context.TrainingSessions.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
