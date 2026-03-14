using BuildingBlocks.CQRS.Requests.Commands;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public record CreateTransactionCommand(
        int UserId,
        int? CategoryId,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false,
        RecurrenceFrequency? Frequency = null,
        DateTime? RecurrenceEndDate = null,
        int? MaxOccurrences = null) : ICommand<CreateTransactionResult>;
    public record CreateTransactionResult(int TransactionId);
}
