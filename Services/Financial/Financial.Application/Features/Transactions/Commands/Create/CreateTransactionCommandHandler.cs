using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public CreateTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<CreateTransactionResult> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateTransactionDTO(
                command.UserId,
                command.CategoryId,
                command.PaymentMethod,
                command.TransactionType,
                command.Amount,
                command.Description,
                command.TransactionDate,
                command.IsRecurring);

            var result = await _transactionService.CreateAsync(dto, cancellationToken);
            return new CreateTransactionResult(result);
        }
    }
}
