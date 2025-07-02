using BuildingBlocks.CQRS.Commands;

namespace Financial.Application.Features.Transactions.Commands.Delete
{
    public record DeleteTransactionCommand(int Id) : ICommand<DeleteTransactionResult>;
    public record DeleteTransactionResult(bool IsSuccess);
}
