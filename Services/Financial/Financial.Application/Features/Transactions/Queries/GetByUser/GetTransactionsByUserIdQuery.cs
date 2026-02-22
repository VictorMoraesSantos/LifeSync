using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetByUserId
{
    public record GetTransactionsByUserIdQuery(int UserId) : IQuery<GetTransactionsByUserIdResult>;
    public record GetTransactionsByUserIdResult(IEnumerable<TransactionDTO> Transactions);
}
