using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Commands.Delete
{
    public record DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, DeleteTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public DeleteTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<DeleteTransactionResult> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = await _transactionService.DeleteAsync(command.Id, cancellationToken);
            return new DeleteTransactionResult(result);
        }
    }
}
