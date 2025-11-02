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
    public class RoutineRepository : IRoutineRepository
    {
        private readonly ApplicationDbContext _context;

        public RoutineRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Routine entity, CancellationToken cancellationToken = default)
        {
            await _context.Routines.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Routine> entities, CancellationToken cancellationToken = default)
        {
            await _context.Routines.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Routine entity, CancellationToken cancellationToken = default)
        {
            _context.Routines.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Routine?>> Find(
            Expression<Func<Routine, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var entities = await _context.Routines
                .Where(predicate)
                .Include(re => re.RoutineExercises)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<(IEnumerable<Routine> Items, int TotalCount)> FindByFilter(RoutineQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new RoutineSpecification(filter);
            IQueryable<Routine> query = _context.Routines.AsNoTracking();
            IQueryable<Routine> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<Routine> finalQuery = SpecificationEvaluator.GetQuery(_context.Routines.AsNoTracking(), spec);
            IEnumerable<Routine> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<Routine?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Routines
                .Include(re => re.RoutineExercises)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<Routine?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Routines
                .Include(re => re.RoutineExercises)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            return entity;
        }

        public async Task Update(Routine entity, CancellationToken cancellationToken = default)
        {
            _context.Routines.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}