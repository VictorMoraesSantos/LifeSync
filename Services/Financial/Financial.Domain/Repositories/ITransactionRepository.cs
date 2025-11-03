using Core.Domain.Repositories;
using Financial.Domain.Entities;
using Financial.Domain.Filters;

namespace Financial.Domain.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, int, TransactionQueryFilter>
    {
        Task<IEnumerable<Transaction?>> GetByUserIdAsync(int userId, DateTime startDate, DateTime endDate, int? categoryId, Enums.TransactionType? type);
    }
}
