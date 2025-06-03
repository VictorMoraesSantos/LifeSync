using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.FinancialAccount;
using System.Runtime.InteropServices;

namespace Financial.Application.Features.FinancialAccounts.Commands.Update
{
    public class UpdateFinancialAccountCommandHandler : IRequestHandler<UpdateFinancialAccountCommand, UpdateFinancialAccountResult>
    {
        private readonly IFinancialAccountService _financialAccountService;

        public UpdateFinancialAccountCommandHandler(IFinancialAccountService financialAccountService)
        {
            _financialAccountService = financialAccountService;
        }

        public async Task<UpdateFinancialAccountResult> Handle(UpdateFinancialAccountCommand command, CancellationToken cancellationToken)
        {
            UpdateFinancialAccountDTO dto = new(command.Id, command.Name, command.AccountType);
            var result = await _financialAccountService.UpdateAsync(dto, cancellationToken);
            return new UpdateFinancialAccountResult(result);
        }
    }
}
