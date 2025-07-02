using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionService _transactionService;

        public CreateTransactionCommandHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<CreateTransactionResult>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
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
            if (!result.IsSuccess)
                return Result.Failure<CreateTransactionResult>(result.Error!);

            return Result.Success(new CreateTransactionResult(result.Value!));
        }
    }
}
