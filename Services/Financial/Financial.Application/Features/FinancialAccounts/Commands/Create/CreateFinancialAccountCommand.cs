using BuildingBlocks.CQRS.Request;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.Application.Features.FinancialAccounts.Commands.Create
{
    public record CreateFinancialAccountCommand(
        int UserId,
        string Name,
        string AccountType,
        Money Balance,
        Currency Currency)
        : IRequest<CreateFinancialAccountResult>;
    public record CreateFinancialAccountResult(int Id);
}
