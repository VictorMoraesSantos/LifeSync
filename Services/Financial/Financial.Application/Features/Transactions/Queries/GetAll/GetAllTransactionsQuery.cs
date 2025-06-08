using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetAll
{
    public record GetAllTransactionsQuery() : IRequest<GetAllTransactionsResult>;
    public record GetAllTransactionsResult(IEnumerable<TransactionDTO> Transactions);
}
