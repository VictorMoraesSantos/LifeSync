using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.FinancialAccount;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetByUserId
{
    public record GetFinancialAccountsByUserIdQuery(int UserId) : IRequest<GetFinancialAccountsByUserResult>;
    public record GetFinancialAccountsByUserResult(IEnumerable<FinancialAccountDTO> FinancialAccounts);
}
