using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
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
        bool IsRecurring = false) : ICommand<CreateTransactionResult>;
    public record CreateTransactionResult(int TransactionId);
}
