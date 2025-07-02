using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.Transactions.Commands.Delete
{
    public record DeleteTransactionCommand(int Id) : ICommand<DeleteTransactionResult>;
    public record DeleteTransactionResult(bool IsSuccess);
}
