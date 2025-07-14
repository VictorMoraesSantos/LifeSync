using Gym.Domain.Entities;
using Gym.Domain.Repositories;
using Gym.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gym.Infrastructure.Persistence.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExerciseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(Exercise entity, CancellationToken cancellationToken = default)
        {
            await _context.Exercises.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<Exercise> entities, CancellationToken cancellationToken = default)
        {
            await _context.Exercises.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(Exercise entity, CancellationToken cancellationToken = default)
        {
            _context.Exercises.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Exercise?>> Find(Expression<Func<Exercise, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _context.Exercises
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<Exercise?>> GetAll(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Exercises
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<Exercise?> GetById(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Exercises
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity;
        }

        public async Task Update(Exercise entity, CancellationToken cancellationToken = default)
        {
            _context.Exercises.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
