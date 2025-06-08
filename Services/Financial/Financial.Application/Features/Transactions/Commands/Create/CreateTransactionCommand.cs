using BuildingBlocks.CQRS.Request;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Features.Transactions.Commands.Create
{
    public record CreateTransactionCommand(
        int UserId,
        int FinancialAccountId,
        int? CategoryId,
        TransactionType Type,
        Money Amount,
        string Description,
        DateTime TransactionDate,
        bool IsRecurring = false) : IRequest<CreateTransactionResult>;
    public record CreateTransactionResult(int TransactionId);
}
