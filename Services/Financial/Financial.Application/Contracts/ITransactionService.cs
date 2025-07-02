using BuildingBlocks.Results;
using Core.Application.Interfaces;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Contracts
{
    public interface ITransactionService
        : IReadService<TransactionDTO, int>,
        ICreateService<CreateTransactionDTO>,
        IUpdateService<UpdateTransactionDTO>,
        IDeleteService<int>
    {
        Task<Result<IEnumerable<TransactionDTO>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    }
}
