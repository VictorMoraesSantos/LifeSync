using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Features.Transactions.Commands.Update
{
    public record UpdateTransactionCommand(
        int Id,
        int? CategoryId,
        PaymentMethod PaymentMethod,
        TransactionType TransactionType,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false)
        : ICommand<UpdateTransactionResult>;
    public record UpdateTransactionResult(bool IsSuccess);
}
