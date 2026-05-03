using LifeSyncApp.DTOs.Financial.RecurrenceSchedule;
using LifeSyncApp.DTOs.Financial.Transaction;

namespace LifeSyncApp.Services.Financial
{
    public interface ITransactionService
    {
        Task<List<TransactionDTO>> GetTransactionsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<TransactionDTO?> GetTransactionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<TransactionDTO>> SearchTransactionsAsync(TransactionFilterDTO filter, CancellationToken cancellationToken = default);
        Task<(int? Id, string? Error)> CreateTransactionAsync(CreateTransactionDTO dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> UpdateTransactionAsync(int id, UpdateTransactionDTO dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string? Error)> DeleteTransactionAsync(int id, CancellationToken cancellationToken = default);
        Task<RecurrenceScheduleDTO?> GetScheduleByTransactionIdAsync(int transactionId, CancellationToken cancellationToken = default);
    }
}
