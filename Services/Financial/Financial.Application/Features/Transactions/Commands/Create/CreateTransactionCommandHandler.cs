using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.DTOs.Transaction;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionService _transactionService;
        private readonly IRecurrenceScheduleService _recurrenceScheduleService;

        public CreateTransactionCommandHandler(ITransactionService transactionService, IRecurrenceScheduleService recurrenceScheduleService)
        {
            _transactionService = transactionService;
            _recurrenceScheduleService = recurrenceScheduleService;
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

            var transactionId = result.Value!;

            if (command.IsRecurring && command.Frequency.HasValue)
            {
                var scheduleDTO = new CreateRecurrenceScheduleDTO(
                    transactionId,
                    command.Frequency.Value,
                    command.TransactionDate,
                    command.RecurrenceEndDate,
                    command.MaxOccurrences);

                var scheduleResult = await _recurrenceScheduleService.CreateAsync(scheduleDTO, cancellationToken);
                if (!scheduleResult.IsSuccess)
                {
                    await _transactionService.DeleteAsync(transactionId, cancellationToken);
                    return Result.Failure<CreateTransactionResult>(scheduleResult.Error!);
                }
            }

            return Result.Success(new CreateTransactionResult(result.Value!));
        }
    }
}
