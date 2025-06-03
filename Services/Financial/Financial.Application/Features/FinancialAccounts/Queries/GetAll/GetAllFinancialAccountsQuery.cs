using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.FinancialAccount;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetAll
{
    public record GetAllFinancialAccountsQuery() : IRequest<GetAllFinancialAccountsResult>;
    public record GetAllFinancialAccountsResult(IEnumerable<FinancialAccountDTO> Accounts);
}
