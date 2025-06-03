using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.FinancialAccounts.Commands.Delete
{
    public record DeleteFinancialAccountCommandHandler : IRequestHandler<DeleteFinancialAccountCommand, DeleteFinancialAccountResult>
    {
        private readonly IFinancialAccountService _financialAccountService;

        public DeleteFinancialAccountCommandHandler(IFinancialAccountService financialAccountService)
        {
            _financialAccountService = financialAccountService;
        }

        public async Task<DeleteFinancialAccountResult> Handle(DeleteFinancialAccountCommand command, CancellationToken cancellationToken)
        {
            var result = await _financialAccountService.DeleteAsync(command.Id, cancellationToken);
            return new DeleteFinancialAccountResult(result);
        }
    }
}
