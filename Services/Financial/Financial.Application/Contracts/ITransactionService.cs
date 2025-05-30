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
    }
}
