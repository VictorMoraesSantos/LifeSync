using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.FinancialAccounts.Queries.GetById
{
    public class GetFinancialAccountByIdQueryHandler : IRequestHandler<GetFinancialAccountByIdQuery, GetFinancialAccountByIdResult>
    {
        private readonly IFinancialAccountService _financialAccount;

        public GetFinancialAccountByIdQueryHandler(IFinancialAccountService financialAccount)
        {
            _financialAccount = financialAccount;
        }

        public async Task<GetFinancialAccountByIdResult> Handle(GetFinancialAccountByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _financialAccount.GetByIdAsync(query.Id, cancellationToken);
            return new GetFinancialAccountByIdResult(result);
        }
    }
}
