using BuildingBlocks.CQRS.Requests.Queries;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetAll
{
    public record GetAllTransactionsQuery() : IQuery<GetAllTransactionsResult>;
    public record GetAllTransactionsResult(IEnumerable<TransactionDTO> Transactions);
}
