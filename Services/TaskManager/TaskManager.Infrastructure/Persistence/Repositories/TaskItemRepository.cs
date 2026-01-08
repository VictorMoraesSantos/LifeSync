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
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetById(int id, CancellationToken cancellationToken = default)
        {
            TaskItem? entity = await _context.TaskItems
                .Include(x => x.Labels)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity;
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> FindByFilter(TaskItemQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new TaskItemSpecification(filter);
            IQueryable<TaskItem> query = _context.TaskItems.AsNoTracking();
            IQueryable<TaskItem> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<TaskItem> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<TaskItem> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskItem?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> entities = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<TaskItem?>> Find(Expression<Func<TaskItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<TaskItem> entities = await _context.TaskItems
                .AsNoTracking()
                .Include(x => x.Labels)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Create(TaskItem entity, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<TaskItem> entities, CancellationToken cancellationToken = default)
        {
            await _context.TaskItems.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Update(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(TaskItem entity, CancellationToken cancellationToken = default)
        {
            _context.TaskItems.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}