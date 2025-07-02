using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetById
{
    public record GetTransactionByIdQuery(int Id) : IQuery<GetTransactionByIdResult>;
    public record GetTransactionByIdResult(TransactionDTO Transaction);
}
