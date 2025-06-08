using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Update
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, UpdateTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public UpdateTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<UpdateTransactionResult> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateTransactionDTO(
                command.Id,
                command.CategoryId,
                command.Type,
                command.Amount,
                command.Description,
                command.TransactionDate);

            var result = await _transactionService.UpdateAsync(dto, cancellationToken);
            return new UpdateTransactionResult(result);
        }
    }
}
