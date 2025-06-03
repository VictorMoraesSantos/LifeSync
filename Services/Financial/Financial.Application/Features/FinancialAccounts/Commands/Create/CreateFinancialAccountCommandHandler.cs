using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.FinancialAccount;

namespace Financial.Application.Features.FinancialAccounts.Commands.Create
{
    public class CreateFinancialAccountCommandHandler : IRequestHandler<CreateFinancialAccountCommand, CreateFinancialAccountResult>
    {
        private readonly IFinancialAccountService _financialAccountService;

        public CreateFinancialAccountCommandHandler(IFinancialAccountService financialAccountService)
        {
            _financialAccountService = financialAccountService;
        }

        public async Task<CreateFinancialAccountResult> Handle(CreateFinancialAccountCommand query, CancellationToken cancellationToken)
        {
            CreateFinancialAccountDTO dto = new(
                query.UserId,
                query.Name,
                query.AccountType,
                query.Balance);

            var result = await _financialAccountService.CreateAsync(dto, cancellationToken);

            return new CreateFinancialAccountResult(result);
        }
    }
}
