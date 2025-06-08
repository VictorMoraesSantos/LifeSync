using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetByUserId
{
    public class GetFinancialAccountsByUserIdQueryHandler : IRequestHandler<GetFinancialAccountsByUserIdQuery, GetFinancialAccountsByUserResult>
    {
        private readonly IFinancialAccountService _financialAccountService;
        
        public GetFinancialAccountsByUserIdQueryHandler(IFinancialAccountService financialAccountService)
        {
            _financialAccountService = financialAccountService;
        }
        public async Task<GetFinancialAccountsByUserResult> Handle(GetFinancialAccountsByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _financialAccountService.GetByUserIdAsync(query.UserId, cancellationToken);
            return new GetFinancialAccountsByUserResult(result);
        }
    }
}
