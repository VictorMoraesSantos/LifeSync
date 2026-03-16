using Core.Domain.Repositories;
using Financial.Domain.Entities;
using Financial.Domain.Filters;

namespace Financial.Domain.Repositories
{
    public interface IRecurrenceScheduleRepository : IRepository<RecurrenceSchedule, int, RecurrenceScheduleQueryFilter>
    {
        Task<RecurrenceSchedule?> GetByTransactionId(int transactionId, CancellationToken cancellationToken);
        Task<IEnumerable<RecurrenceSchedule>> GetActiveByUserId(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<RecurrenceSchedule>> GetDueSchedules(DateTime referenceDate, CancellationToken cancellationToken);
    }
}
