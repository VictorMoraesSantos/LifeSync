using BuildingBlocks.CQRS.Requests.Queries;
using Core.Infrastructure.Persistence.Specifications;
using Financial.Domain.Entities;
using Financial.Domain.Filters;
using Financial.Domain.Filters.Specifications;
using Financial.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Linq.Expressions;

namespace Financial.Infrastructure.Persistence.Repositories
{
    public class RecurrenceScheduleRepository : IRecurrenceScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public RecurrenceScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Create(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            await _context.RecurrenceSchedule.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateRange(IEnumerable<RecurrenceSchedule> entities, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            entity.MarkAsDeleted();
            _context.RecurrenceSchedule.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecurrenceSchedule?>> Find(Expression<Func<RecurrenceSchedule, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IEnumerable<RecurrenceSchedule> entities = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .Where(predicate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<(IEnumerable<RecurrenceSchedule> Items, int TotalCount)> FindByFilter(RecurrenceScheduleQueryFilter filter, CancellationToken cancellationToken = default)
        {
            var spec = new RecurrenceScheduleSpecification(filter);
            IQueryable<RecurrenceSchedule> query = _context.RecurrenceSchedule.AsNoTracking();
            IQueryable<RecurrenceSchedule> countQuery = spec.Criteria != null ? query.Where(spec.Criteria) : query;
            int totalCount = await countQuery.CountAsync(cancellationToken);
            IQueryable<RecurrenceSchedule> finalQuery = SpecificationEvaluator.GetQuery(query, spec);
            IEnumerable<RecurrenceSchedule> items = await finalQuery.ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IEnumerable<RecurrenceSchedule>> GetActiveByUserId(int userId, CancellationToken cancellationToken)
        {
            IEnumerable<RecurrenceSchedule> entities = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .Where(r => r.Transaction.UserId == userId && r.IsActive)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<IEnumerable<RecurrenceSchedule?>> GetAll(CancellationToken cancellationToken = default)
        {
            IEnumerable<RecurrenceSchedule> entities = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task<RecurrenceSchedule?> GetById(int id, CancellationToken cancellationToken = default)
        {
            RecurrenceSchedule? entity = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            return entity;
        }

        public async Task<RecurrenceSchedule> GetByTransactionId(int transactionId, CancellationToken cancellationToken)
        {
            RecurrenceSchedule? entity = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(r => r.TransactionId == transactionId, cancellationToken);

            return entity;
        }

        public async Task<IEnumerable<RecurrenceSchedule>> GetDueSchedules(DateTime referenceDate, CancellationToken cancellationToken)
        {
            IEnumerable<RecurrenceSchedule> entities = await _context.RecurrenceSchedule
                .Include(r => r.Transaction)
                    .ThenInclude(t => t.Category)
                .Where(r => r.IsActive && !r.IsDeleted && r.NextOccurrence <= referenceDate)
                .ToListAsync(cancellationToken);

            return entities;
        }

        public async Task Update(RecurrenceSchedule entity, CancellationToken cancellationToken = default)
        {
            _context.RecurrenceSchedule.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
