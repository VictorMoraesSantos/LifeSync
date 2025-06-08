using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Queries.GetById
{
    public record GetTransactionByIdQuery(int Id) : IRequest<GetTransactionByIdResult>;
    public record GetTransactionByIdResult(TransactionDTO Transaction);
}
