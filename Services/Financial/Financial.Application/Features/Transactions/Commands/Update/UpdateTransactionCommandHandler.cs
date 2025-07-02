using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Update
{
    public class UpdateTransactionCommandHandler : ICommandHandler<UpdateTransactionCommand, UpdateTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public UpdateTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<Result<UpdateTransactionResult>> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateTransactionDTO(
                command.Id,
                command.CategoryId,
                command.PaymentMethod,
                command.TransactionType,
                command.Amount,
                command.Description,
                command.TransactionDate);

            var result = await _transactionService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<UpdateTransactionResult>(result.Error!);

            return Result.Success(new UpdateTransactionResult(result.Value!));
        }
    }
}
