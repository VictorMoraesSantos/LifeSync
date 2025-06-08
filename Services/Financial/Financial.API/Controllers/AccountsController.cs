using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.Features.FinancialAccounts.Commands.Create;
using Financial.Application.Features.FinancialAccounts.Commands.Delete;
using Financial.Application.Features.FinancialAccounts.Commands.Update;
using Financial.Application.Features.FinancialAccounts.Queries.GetAll;
using Financial.Application.Features.FinancialAccounts.Queries.GetById;
using Financial.Application.Features.FinancialAccounts.Queries.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace Financial.API.Controllers
{
    public class AccountsController : ApiController
    {
        private readonly ISender _sender;

        public AccountsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetFinancialAccountByIdResult>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetFinancialAccountByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetFinancialAccountByIdResult>.Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetFinancialAccountsByUserResult>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var query = new GetFinancialAccountsByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetFinancialAccountsByUserResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllFinancialAccountsResult>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllFinancialAccountsQuery();
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetAllFinancialAccountsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateFinancialAccountResult>> CreateAsync([FromBody] CreateFinancialAccountCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<CreateFinancialAccountResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateFinancialAccountResult>> UpdateAsync(int id, [FromBody] UpdateFinancialAccountCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateFinancialAccountCommand(id, command.Name, command.AccountType);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return HttpResult<UpdateFinancialAccountResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteFinancialAccountResult>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteFinancialAccountCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<DeleteFinancialAccountResult>.Ok(result);
        }
    }
}
