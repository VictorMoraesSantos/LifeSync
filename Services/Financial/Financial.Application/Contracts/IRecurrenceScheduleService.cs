using BuildingBlocks.Results;
using Core.Application.Interfaces;
using Financial.Application.DTOs.RecurrenceSchedule;

namespace Financial.Application.Contracts
{
    public interface IRecurrenceScheduleService
        : IReadService<RecurrenceScheduleDTO, int, RecurrenceScheduleFilterDTO>,
        ICreateService<CreateRecurrenceScheduleDTO>,
        IUpdateService<UpdateRecurrenceScheduleDTO>,
        IDeleteService<int>
    {
        Task<Result<RecurrenceScheduleDTO>> GetByTransactionIdAsync(int transactionId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<RecurrenceScheduleDTO>>> GetActiveByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeactiveScheduleAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<bool>> ActiveScheduleAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<int>> ProcessDueSchedulesAsync(CancellationToken cancellationToken = default);
    }
}
