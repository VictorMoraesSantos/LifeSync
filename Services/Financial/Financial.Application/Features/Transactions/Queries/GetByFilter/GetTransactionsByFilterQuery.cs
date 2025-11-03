using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.Results;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetByFilter
{
    public record GetTransactionsByFilterQuery(TransactionFilterDTO Filter) : IQuery<GetTransactionsByFilterResult>;
    public record GetTransactionsByFilterResult(IEnumerable<TransactionDTO> Items, PaginationData Pagination);
}
