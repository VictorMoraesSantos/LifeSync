using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetByUserId
{
    public record GetTransactionsByUserIdQuery(int UserId) : IQuery<GetTransactionsByUserIdResult>;
    public record GetTransactionsByUserIdResult(IEnumerable<TransactionDTO> Transactions);
}
