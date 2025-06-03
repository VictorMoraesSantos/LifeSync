using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.FinancialAccounts.Commands.Delete
{
    public record DeleteFinancialAccountCommand(int Id) : IRequest<DeleteFinancialAccountResult>;
    public record DeleteFinancialAccountResult(bool IsSuccess);
}
