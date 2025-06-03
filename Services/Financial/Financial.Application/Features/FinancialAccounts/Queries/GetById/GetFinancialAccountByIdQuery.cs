using BuildingBlocks.CQRS.Request;
using Financial.Application.DTOs.FinancialAccount;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetById
{
    public record GetFinancialAccountByIdQuery(int Id) : IRequest<GetFinancialAccountByIdResult>;
    public record GetFinancialAccountByIdResult(FinancialAccountDTO FinancialAccount);
}
