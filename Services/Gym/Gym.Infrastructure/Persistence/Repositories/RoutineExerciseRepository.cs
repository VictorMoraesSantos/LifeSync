using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using Gym.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class RoutineExerciseRepository : IRoutineExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public RoutineExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            await _context.RoutineExercises.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<RoutineExercise> entities, CancellationToken cancellationToken = default)
        {
            await _context.RoutineExercises.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            _context.RoutineExercises.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<RoutineExercise?>> Find(
            Expression<Func<RoutineExercise, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var entities = await _context.RoutineExercises
                .Where(predicate)
                .Include(e => e.Routine)
                .Include(e => e.Exercise)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<RoutineExercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.RoutineExercises
                .Include(e => e.Routine)
                .Include(e => e.Exercise)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<RoutineExercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entities = await _context.RoutineExercises
                .AsNoTracking()
                .Include(e => e.Routine)
                .Include(e => e.Exercise)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            return entities;
        }

        public async Task Update(RoutineExercise entity, CancellationToken cancellationToken = default)
        {
            _context.RoutineExercises.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}