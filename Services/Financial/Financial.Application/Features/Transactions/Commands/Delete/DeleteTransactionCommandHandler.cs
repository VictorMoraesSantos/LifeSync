using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Commands.Delete
{
    public record DeleteTransactionCommandHandler : ICommandHandler<DeleteTransactionCommand, DeleteTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public DeleteTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<DeleteTransactionResult>> Handle(DeleteTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = await _transactionService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<DeleteTransactionResult>(result.Error!);

            return Result.Success(new DeleteTransactionResult(result.Value!));
        }
    }
}
