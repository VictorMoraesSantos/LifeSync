using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetAll
{
    public class GetAllFinancialAccountsQueryHandler : IRequestHandler<GetAllFinancialAccountsQuery, GetAllFinancialAccountsResult>
    {
        private readonly IFinancialAccountService _financialAccountService;

        public GetAllFinancialAccountsQueryHandler(IFinancialAccountService financialAccountService)
        {
            _financialAccountService = financialAccountService;
        }

        public async Task<GetAllFinancialAccountsResult> Handle(GetAllFinancialAccountsQuery query, CancellationToken cancellationToken)
        {
            var result = await _financialAccountService.GetAllAsync(cancellationToken);
            return new GetAllFinancialAccountsResult(result);
        }
    }
}
