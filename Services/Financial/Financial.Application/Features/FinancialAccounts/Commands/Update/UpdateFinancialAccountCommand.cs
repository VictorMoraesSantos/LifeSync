using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.FinancialAccounts.Commands.Update
{
    public record UpdateFinancialAccountCommand(int Id, string Name, string AccountType) : IRequest<UpdateFinancialAccountResult>;
    public record UpdateFinancialAccountResult(bool IsSuccess);
}
