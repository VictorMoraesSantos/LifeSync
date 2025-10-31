using Core.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Filters;
using TaskManager.Domain.Filters.Specifications;
using TaskManager.Domain.Repositories;
using TaskManager.Infrastructure.Persistence.Data;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class TaskLabelRepository : ITaskLabelRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskLabelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskLabel?> GetById(int id, CancellationToken cancellationToken = default)
        {
            TaskLabel? entity = await _context.TaskLabels
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity;
        }

        public async Task<(IEnumerable<TaskLabel> Items, int TotalCount)> FindByFilter(TaskLabelFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new TaskLabelSpecification(filter);
            IQueryable<TaskLabel> query = _context.TaskLabels.AsNoTracking();
            IQueryable<TaskLabel> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<TaskLabel> finalQuery = SpecificationEvaluator.GetQuery(_context.TaskLabels.AsNoTracking(), spec);
            IEnumerable<TaskLabel> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskLabel?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> entities = await _context.TaskLabels
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<TaskLabel?>> Find(Expression<Func<TaskLabel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskLabel> entities = await _context.TaskLabels
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            await _context.TaskLabels.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskLabel> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskLabels.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskLabel entity, CancellationToken cancellationToken = default)
        {
            _context.TaskLabels.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
