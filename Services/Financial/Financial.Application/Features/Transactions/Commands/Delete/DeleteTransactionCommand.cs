using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.Transactions.Commands.Delete
{
    public record DeleteTransactionCommand(int Id) : IRequest<DeleteTransactionResult>;
    public record DeleteTransactionResult(bool IsSuccess);
}
